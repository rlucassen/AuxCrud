﻿namespace AuxCrud.Web
{
    #region Usings

    using System;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using Castle.MicroKernel.Registration;
    using Castle.MonoRail.Framework;
    using Castle.MonoRail.Framework.Configuration;
    using Castle.MonoRail.Framework.Container;
    using Castle.MonoRail.Framework.Helpers.ValidationStrategy;
    using Castle.MonoRail.Framework.JSGeneration;
    using Castle.MonoRail.Framework.JSGeneration.jQuery;
    using Castle.MonoRail.Framework.Routing;
    using Castle.Windsor;
    using Controllers;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using Model;
    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Context;
    using NHibernate.Tool.hbm2ddl;
    using NHibernate.Validator.Cfg;
    using NHibernate.Validator.Engine;
    using NHibernate.Validator.Event;
    using AllDefinitions = NHibernate.Validator.Cfg.Loquacious.AllDefinitions;
    using Environment = NHibernate.Validator.Cfg.Environment;

    #endregion

    public static class RoutingEngineExtension
    {
        public static void Add<T>(this RoutingEngine engine)
            where T : class, IController
        {
            string name = typeof (T).Name.Replace("Controller", string.Empty);
            var controllerDetails =
                typeof (T).GetCustomAttributes(typeof (ControllerDetailsAttribute), true).FirstOrDefault() as
                ControllerDetailsAttribute;
            string area = string.Empty;
            if (controllerDetails != null)
            {
                area = controllerDetails.Area;
            }

            PatternRoute patternIdRoute =
                new PatternRoute(string.Format("{1}/{0}/<id>/<action>", name,
                                               string.IsNullOrEmpty(area) ? string.Empty : string.Format("/{0}", area)))
                    .DefaultForController().Is<T>()
                    .Restrict("id").ValidInteger
                    .DefaultForAction().Is("index");

            PatternRoute patternActionRoute =
                new PatternRoute(string.Format("{1}/{0}/<action>", name,
                                               string.IsNullOrEmpty(area) ? string.Empty : string.Format("/{0}", area)))
                    .DefaultForController().Is<T>()
                    .DefaultForAction().Is("index");

            PatternRoute patternRoute =
                new PatternRoute(string.Format("{1}/{0}", name,
                                               string.IsNullOrEmpty(area) ? string.Empty : string.Format("/{0}", area)))
                    .DefaultForController().Is<T>()
                    .DefaultForAction().Is("index");

            if (!string.IsNullOrEmpty(area))
            {
                patternIdRoute.DefaultForArea().Is(area);
                patternActionRoute.DefaultForArea().Is(area);
                patternRoute.DefaultForArea().Is(area);
            }
            engine.Add(patternIdRoute);
            engine.Add(patternActionRoute);
            engine.Add(patternRoute);
        }
    }

    public class Global : HttpApplication, IContainerAccessor, IMonoRailContainerEvents, IMonoRailConfigurationEvents
    {
        private static IWindsorContainer container;

        #region IContainerAccessor Members

        public IWindsorContainer Container
        {
            get { return container; }
        }

        #endregion

        #region IMonoRailConfigurationEvents Members

        public void Configure(IMonoRailConfiguration configuration)
        {
            configuration.JSGeneratorConfiguration.AddLibrary("jquery-1.8.2", typeof (JQueryGenerator)).AddExtension(
                typeof (CommonJSExtension)).ElementGenerator.AddExtension(typeof (JQueryElementGenerator)).Done.
                BrowserValidatorIs(typeof (JQueryValidator)).SetAsDefault();
        }

        #endregion

        #region IMonoRailContainerEvents Members

        public void Created(IMonoRailContainer container)
        {
        }

        public void Initialized(IMonoRailContainer container)
        {
        }

        #endregion

        protected void Application_Start(object sender, EventArgs e)
        {
            container = new AuxCrudContainer();
            RegisterSessionFactory(container);

            RoutingModuleEx.Engine.Add(new PatternRoute("/")
                                           .DefaultForController().Is<Controllers.HomeController>()
                                           .DefaultForAction().Is("index"));

            RoutingModuleEx.Engine.Add(
                new PatternRoute("/<controller>")
                    .DefaultForController().Is("home")
                    .DefaultForAction().Is("index")
            );

            RoutingModuleEx.Engine.Add(
                new PatternRoute("/<controller>/<action>")
                    .DefaultForController().Is("home")
                    .DefaultForAction().Is("index")
            );

            RoutingModuleEx.Engine.Add(
                new PatternRoute("/<controller>/<id>/<action>")
                    .DefaultForController().Is("home")
                    .DefaultForAction().Is("index")
                    .Restrict("id").ValidInteger
            );

            RoutingModuleEx.Engine.Add(
                new PatternRoute("/<area>/<controller>/<action>")
                    .DefaultForArea().Is("admin")
                    .DefaultForController().Is("home")
                    .DefaultForAction().Is("index")
            );

            RoutingModuleEx.Engine.Add(
                new PatternRoute("/<area>/<controller>/<id>/<action>")
                    .DefaultForArea().Is("admin")
                    .DefaultForController().Is("home")
                    .DefaultForAction().Is("index")
                    .Restrict("id").ValidInteger
            );
        }

        protected void Application_End(object sender, EventArgs e)
        {
            if (container != null) container.Dispose();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var sessionFactory = Container.Resolve<ISessionFactory>();
            ISession session = sessionFactory.OpenSession();
            ManagedWebSessionContext.Bind(HttpContext.Current, session);
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            var sessionFactory = Container.Resolve<ISessionFactory>();
            ISession session = ManagedWebSessionContext.Unbind(HttpContext.Current, sessionFactory);
            if (session != null && session.IsOpen) session.Close();
        }

        protected static void RegisterSessionFactory(IWindsorContainer windsorContainer)
        {
            FluentConfiguration configuration = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(c => c.FromConnectionStringWithKey("AuxCrud")))
                .ExposeConfiguration(c => c.CurrentSessionContext<ManagedWebSessionContext>())
                .ExposeConfiguration(c => ConfigureValidatorEngine<ModelBase>(c))
                .ExposeConfiguration(ExportSchema)
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ModelBase>());

            ISessionFactory sessionFactory = configuration.BuildSessionFactory();
            windsorContainer.Register(Component.For<ISessionFactory>().Instance(sessionFactory));
        }

        private static void ExportSchema(Configuration configuration)
        {
            new SchemaUpdate(configuration).Execute(true, true);
            //var scriptGenerator = new SchemaExport(configuration);
            //scriptGenerator.SetOutputFile(@"C:\AppOnline.sql");
            //scriptGenerator.Execute(true, false, false);
        }

        private static ValidatorEngine ConfigureValidatorEngine<T>(Configuration configuration)
        {
            Environment.SharedEngineProvider = new NHibernateSharedEngineProvider();
            ValidatorEngine validatorEngine = Environment.SharedEngineProvider.GetEngine();
            var validatorConfiguration = new NHibernate.Validator.Cfg.Loquacious.FluentConfiguration();
            validatorConfiguration.SetDefaultValidatorMode(ValidatorMode.UseAttribute)
                .Register(AllDefinitions.ValidationDefinitions(typeof (T).Assembly))
                .IntegrateWithNHibernate
                .ApplyingDDLConstraints()
                .And
                .RegisteringListeners();
            validatorEngine.Configure(validatorConfiguration);

            configuration.Initialize(validatorEngine);
            return validatorEngine;
        }
    }
}