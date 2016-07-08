namespace AuxCrud.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Security;
    using Castle.MonoRail.Framework;
    using Model;
    using Model.Helpers;
    using NHibernate;
    using NHibernate.Linq;
    using ViewModel.Attributes;
    using ViewModel.Forms;
    using ViewModel.Helpers;
    using ViewModel.ViewModel;

    public abstract class CrudController<TOwner, TViewModel> : BaseController where TOwner : ModelBase, new() where TViewModel : ViewModel<TOwner, TViewModel>
    {
        protected CrudController(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public virtual bool NewEnabled => true;
        public virtual bool QuickviewEnabled => true;
        public virtual bool EditEnabled => true;
        public virtual bool DeleteEnabled => true;
        public virtual string QuickviewAction => "view";

        public void Index()
        {
            FillPropertyBagDefaults();
            RenderView("/Crud/index");
        }

        public void New()
        {
            if (!NewEnabled)
                throw new SecurityException("No new objects allowed");
            var item = (TViewModel)Activator.CreateInstance(typeof(TViewModel), new TOwner());
            RenderEdit(item);
        }

        public void View(int id)
        {
            FillPropertyBagDefaults();
            PropertyBag.Add("item", Activator.CreateInstance(typeof(TViewModel), session.Get<TOwner>(id)));
        }

        public void Edit(int id)
        {
            if(!EditEnabled)
                throw new SecurityException("No edits allowed");
            var item = (TViewModel)Activator.CreateInstance(typeof(TViewModel), session.Get<TOwner>(id));
            RenderEdit(item);
        }

        private void RenderEdit(TViewModel dto)
        {
            FillPropertyBagDefaults();
            PropertyBag.Add("item", dto);
            PropertyBag.Add("crudForm", FormParser.Parse(dto.FormComponent, session));
            RenderView("/Crud/edit");
        }

        [return: JSONReturnBinder]
        public ListReturn<TOwner, TViewModel> List(int start, int length, int draw, string orderColumn, string orderDir, string searchValue)
        {
            var projectData = session.Query<TOwner>().Where(x => x.IsActive);
            var recordsTotal = projectData.Count();

            if (!string.IsNullOrEmpty(orderColumn))
            {
                if (!typeof(TOwner).PropertyTreeExists(orderColumn))
                {
                    throw new Exception($"Object {typeof(TOwner).Name} has no propertytree {orderColumn}");
                }
                projectData = projectData.OrderByProperty(orderColumn, orderDir != "asc");
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                var searchExpressions = ((TViewModel) Activator.CreateInstance(typeof (TViewModel), new TOwner())).Maps.Where(x => x.Searchable).Select(x => x.OwnerExpression).ToArray();
                projectData = projectData.WherePropertiesContain(searchExpressions, searchValue);
            }

            projectData = projectData.Skip(start).Take(length);
            var recordsFiltered = projectData.Count();
            var data = projectData.ToList().Select(x => (TViewModel)Activator.CreateInstance(typeof(TViewModel), x)).ToList();
            return new ListReturn<TOwner, TViewModel>(draw, start, length, recordsTotal, recordsFiltered, data);
        }

        public void Save(int id)
        {
            var dto = BindViewModel("item");

            if (!dto.IsValid())
            {
                PropertyBag.Add("errors", dto.Errors());
                RenderEdit(dto);
                return;
            }

            var item = dto.Update(session);

            using (var tx = session.BeginTransaction())
            {
                session.SaveOrUpdate(item);
                tx.Commit();
            }

            RedirectToAction("index");
        }

        public TViewModel BindViewModel(string prefix)
        {
            var viewModel = (TViewModel)Activator.CreateInstance(typeof(TViewModel), new TOwner());
            BindObjectInstance(viewModel, prefix);
            viewModel.BindReferences(session, Params);
            return viewModel;
        }

        public void Delete(int id)
        {
            var item = session.Get<TOwner>(id);

            item.Deactivate();

            using (var tx = session.BeginTransaction())
            {
                session.SaveOrUpdate(item);
                tx.Commit();
            }

            RedirectToAction("index");
        }

        [return: JSONReturnBinder]
        public IList<TableColumn> Columns()
        {
            var fields = typeof(TViewModel).GetProperties().Where(pi => pi.GetCustomAttributes(typeof(TableColumnAttribute), false).Any());
            var columns = new List<TableColumn>();

            if (QuickviewEnabled)
            {
                columns.Add(new TableColumn()
                {
                    columnOrder = 0,
                    orderable = false,
                    defaultContent = "<i class=\"fa fa-minus-square red\"></i><i class=\"fa fa-plus-square green\"></i>",
                    className = "details-control"

                });
            }

            foreach (var field in fields)
            {
                var attribute = (TableColumnAttribute)field.GetCustomAttributes(typeof (TableColumnAttribute), false).First();
                var data = field.Name;
                //if (field.PropertyType.BaseType != null && field.PropertyType.BaseType.IsGenericType && field.PropertyType.BaseType.GetGenericTypeDefinition() == typeof(ViewModel<TOwner, TViewModel>).GetGenericTypeDefinition())
                //{
                //    data = $"{field.Name}.Readable";
                //}
                var column = new TableColumn()
                {
                    columnOrder = attribute.Order,
                    data = data,
                    name = attribute.MappingField,
                    orderable = attribute.Orderable,
                    className = attribute.ClassName,
                    title = string.IsNullOrEmpty(attribute.Title) ? field.Name : attribute.Title
                };
                columns.Add(column);
            }

            if (EditEnabled || DeleteEnabled)
            {
                var actions = "";
                if (EditEnabled)
                {
                    actions += $"<a class=\"button tiny success no-margin edit\" href=\"#\">{Localization.Language.Form_Edit}</a>";
                }
                if (DeleteEnabled)
                {
                    actions += $"<a class=\"button tiny alert no-margin delete\" href=\"#\">{Localization.Language.Form_Delete}</a>";
                }
                columns.Add(new TableColumn()
                {
                    columnOrder = 99,
                    orderable = false,
                    defaultContent = actions,
                    className = "actions"
                });
            }

            return columns.OrderBy(x => x.columnOrder).ToList();
        }

        protected abstract void FillPropertyBag();

        protected virtual void FillPropertyBagDefaults()
        {
            PropertyBag.Add("newEnabled", NewEnabled);
            PropertyBag.Add("quickviewAction", QuickviewAction);
            PropertyBag.Add("controllerName", GetType().Name.Replace("Controller", ""));
            var viewModelAttribute = (ViewModelAttribute)typeof (TViewModel).GetCustomAttributes(typeof (ViewModelAttribute), false).FirstOrDefault();
            var viewModelName = viewModelAttribute?.Name ?? typeof(TViewModel).Name;
            PropertyBag.Add("viewModelName", viewModelName.ToLower());
            PropertyBag.Add("viewModelNameCap", viewModelName.Capitalize());
            var viewModelPluralName = viewModelAttribute?.PluralName ?? typeof(TViewModel).Name;
            PropertyBag.Add("viewModelPluralName", viewModelPluralName.ToLower());
            PropertyBag.Add("viewModelPluralNameCap", viewModelPluralName.Capitalize());
            FillPropertyBag();
        }
    }


    public class TableColumn
    {
        public int columnOrder;
        public string data;
        public string name;
        public bool orderable = true;
        public string className;
        public string defaultContent;
        public string title = "";
    }

    public class ListReturn<TOwner, TViewModel> where TOwner : ModelBase, new() where TViewModel : ViewModel<TOwner, TViewModel>
    {
        public ListReturn(int draw, int start, int length, int recordsTotal, int recordsFiltered, IList<TViewModel> data)
        {
            this.draw = draw;
            this.start = start;
            this.length = length;
            this.recordsTotal = recordsTotal;
            this.recordsFiltered = recordsFiltered;
            this.data = data;
        }

        public int draw;
        public int start;
        public int length;
        public int recordsTotal;
        public int recordsFiltered;
        public IList<TViewModel> data;
    }

}