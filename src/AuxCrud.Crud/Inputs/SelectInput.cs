namespace AuxCrud.ViewModel.Inputs
{
    using System;
    using System.Linq;
    using Interfaces;
    using Model;
    using NHibernate;
    using NHibernate.Criterion;

    public class SelectInput : BaseInput
    {
        public SelectInput() : base(false)
        {
        }

        public SelectInput(bool required) : base(required)
        {
        }


        public override string Render(string property, object value, string name, ISession session)
        {
            var objectDtoType = value.GetType();
            var objectType = objectDtoType.BaseType?.GetGenericArguments().First(x => x.BaseType == typeof (ModelBase));
            var id = ((IViewModel) value).Id;
            var selectedObject = (ModelBase)session.Get(objectType, id);

            var list = session.CreateCriteria(objectType).Add(Restrictions.Eq("IsActive", true)).List();
            var modelBases = list.OfType<ModelBase>();
            var enumerable = modelBases.Select(x => Activator.CreateInstance(objectDtoType, x));
            var objectOptions = enumerable.OfType<IViewModel>();

            var input = $"<select name=\"item.{property}\" pattern=\"{Patterns.NoZero}\">";

            input += $"<option value=\"0\" {(selectedObject == null ? "selected" : "")}>Please choose</option>";

            foreach (var objectOption in objectOptions)
            {
                var selected = selectedObject != null && selectedObject.Id == objectOption.Id ? "selected" : "";
                input += $"<option value=\"{objectOption.Id}\" {selected}>{objectOption.Readable}</option>";
            }

            input += "</select>";

            var messageElem = Required ? $"<small class=\"error\">{GetMessage(name)}</small>" : "";
            return $"{input}{messageElem}";
        }

        public override bool Isvalid(object value)
        {
            return !Required || ((IViewModel)value).Id > 0;
        }
    }
}