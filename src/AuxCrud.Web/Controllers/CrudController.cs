namespace AuxCrud.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using Castle.MonoRail.Framework;
    using Model;
    using Model.Helpers;
    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.Linq;
    using ViewModel.Attributes;
    using ViewModel.Helpers;
    using ViewModel.ViewModel;

    public abstract class CrudController<T, TDto> : BaseController where T : ModelBase, new() where TDto : ViewModel<T>, new()
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
            var item = new TDto();
            RenderEdit(item);
        }

        public void View(int id)
        {
            FillPropertyBagDefaults();
            PropertyBag.Add("item", Activator.CreateInstance(typeof(TDto), session.Get<T>(id)));
        }

        public void Edit(int id)
        {
            if(!EditEnabled)
                throw new SecurityException("No edits allowed");
            var item = (TDto)Activator.CreateInstance(typeof(TDto), session.Get<T>(id));
            RenderEdit(item);
        }

        private void RenderEdit(TDto dto)
        {
            FillPropertyBagDefaults();
            PropertyBag.Add("item", dto);
            PropertyBag.Add("inputs", dto.Inputs(session));
            RenderView("/Crud/edit");
        }

        [return: JSONReturnBinder]
        public ListReturn<T, TDto> List(int start, int length, int draw, string orderColumn, string orderDir, string searchValue)
        {
            var projectData = session.Query<T>().Where(x => x.IsActive);
            var recordsTotal = projectData.Count();

            if (!string.IsNullOrEmpty(orderColumn))
            {
                if (!typeof(T).PropertyTreeExists(orderColumn))
                {
                    throw new Exception($"Object {typeof(T).Name} has no propertytree {orderColumn}");
                }
                projectData = projectData.OrderByProperty(orderColumn, orderDir != "asc");
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                projectData = projectData.WherePropertiesContain(GetSearchProperties(), searchValue);
            }

            projectData = projectData.Skip(start).Take(length);
            var recordsFiltered = projectData.Count();
            var data = projectData.ToList().Select(x => (TDto)Activator.CreateInstance(typeof(TDto), x)).ToList();
            return new ListReturn<T, TDto>(draw, start, length, recordsTotal, recordsFiltered, data);
        }

        public void Save(int id)
        {
            var dto = BindObject<TDto>("item");

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

        public void Delete(int id)
        {
            var item = session.Get<T>(id);

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
            var fields = typeof (TDto).GetProperties().Where(pi => pi.GetCustomAttributes(typeof (TableColumnAttribute), false).Length > 0);
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
                var mappingAttribute = (MappingAttribute)field.GetCustomAttributes(typeof (MappingAttribute), false).FirstOrDefault();
                var column = new TableColumn()
                {
                    columnOrder = attribute.Order,
                    data = field.Name,
                    name = mappingAttribute?.PropertyTree ?? attribute.MappingField ?? field.Name,
                    orderable = attribute.Orderable,
                    className = attribute.ClassName,
                    title = attribute.Title ?? field.Name
                };
                columns.Add(column);
            }

            if (EditEnabled || DeleteEnabled)
            {
                var actions = "";
                if (EditEnabled)
                {
                    actions += "<a class=\"button tiny success no-margin edit\" href=\"#\">Bewerk</a>";
                }
                if (DeleteEnabled)
                {
                    actions += "<a class=\"button tiny alert no-margin delete\" href=\"#\">Verwijderen</a>";
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

        private static string[] GetSearchProperties()
        {
            var fields = typeof (TDto).GetProperties().Where(pi => pi.GetCustomAttributes(typeof (SearchFieldAttribute), false).Length > 0);
            var properties = new List<string>();

            foreach (var field in fields)
            {
                var attribute = (MappingAttribute) field.GetCustomAttributes(typeof (MappingAttribute), false).First();
                var fieldName = attribute.PropertyTree;
                
                if (!typeof(T).PropertyTreeExists(fieldName))
                {
                    throw new Exception($"Field {field.Name} has no field of type string named {fieldName} in the original object");
                }
                properties.Add(fieldName);
            }
            return properties.ToArray();
        }

        protected abstract void FillPropertyBag();

        protected virtual void FillPropertyBagDefaults()
        {
            PropertyBag.Add("newEnabled", NewEnabled);
            PropertyBag.Add("quickviewAction", QuickviewAction);
            PropertyBag.Add("controllerName", GetType().Name.Replace("Controller", ""));
            var viewModelAttribute = (ViewModelAttribute)typeof (TDto).GetCustomAttributes(typeof (ViewModelAttribute), false).FirstOrDefault();
            var viewModelName = viewModelAttribute?.Name ?? typeof(TDto).Name;
            PropertyBag.Add("viewModelName", viewModelName);
            PropertyBag.Add("viewModelNameCap", viewModelName.Capitalize());
            var viewModelPluralName = viewModelAttribute?.PluralName ?? typeof(TDto).Name;
            PropertyBag.Add("viewModelPluralName", viewModelPluralName);
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

    public class ListReturn<T, TDto> where T : ModelBase, new() where TDto : ViewModel<T>
    {
        public ListReturn(int draw, int start, int length, int recordsTotal, int recordsFiltered, IList<TDto> data)
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
        public IList<TDto> data;
    }

}