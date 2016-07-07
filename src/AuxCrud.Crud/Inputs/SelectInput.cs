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
        public SelectInput(Type objectType) : base(false)
        {
            ObjectType = objectType;
        }

        public SelectInput(Type objecType, bool required) : base(required)
        {
            ObjectType = objecType;
        }

        public Type ObjectType { get; set; }
        public Type ObjectDtoType { get; set; }

        public override string Render(string property, object value, string name, ISession session)
        {
            var selectedObject = (ModelBase)session.Get(ObjectType, value);

            var list = session.CreateCriteria(ObjectType).Add(Restrictions.Eq("IsActive", true)).List();
            var modelBases = list.OfType<ModelBase>();
            var enumerable = modelBases.Select(x => Activator.CreateInstance(ObjectDtoType, x));
            var objectOptions = enumerable.OfType<IListable>();

            var input = $"<select name=\"item.{property}\">";

            input += "<option>Please choose";

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
            var intValue = Convert.ToInt32(value);
            return !Required || intValue > 0;
        }
    }
}