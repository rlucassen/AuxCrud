﻿namespace AuxCrud.Web.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Castle.MonoRail.Framework;
    using Castle.MonoRail.Framework.Filters;
    using NHibernate;

    #endregion

    [Layout("default")]
    [Resource("t", "AuxCrud.Localization.Language", AssemblyName = "AuxCrud.Localization")]
    [Filter(ExecuteWhen.BeforeAction, typeof(RequestValidatorFilter), ExecutionOrder = int.MinValue)]
    public abstract class BaseController : SmartDispatcherController
    {
        protected ISession session;

        protected BaseController(ISessionFactory sessionFactory)
        {
            session = sessionFactory.GetCurrentSession();
        }

        protected void Error(string message, bool redirectToReferrer = false)
        {
            CreateMessage("error", message, redirectToReferrer);
        }

        protected void Information(string message, bool redirectToReferrer = false)
        {
            CreateMessage("info", message, redirectToReferrer);
        }

        protected void Success(string message, bool redirectToReferrer = false)
        {
            CreateMessage("success", message, redirectToReferrer);
        }

        private void CreateMessage(string type, string message, bool redirectToReferrer)
        {
            Flash[type] = message;
            if(redirectToReferrer) 
                RedirectToReferrer();
        }

        protected override object InvokeMethod(MethodInfo method, IRequest request, IDictionary<string, object> extraArgs)
        {
            if (Convert.ToBoolean(request.Params["cancelLayout"]))
            {
                CancelLayout();
            }
            return base.InvokeMethod(method, request, extraArgs);
        }

    }
}