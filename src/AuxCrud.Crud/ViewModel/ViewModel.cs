namespace AuxCrud.ViewModel.ViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Attributes;
    using Helpers;
    using Inputs;
    using Interfaces;
    using Model;
    using NHibernate;

    public abstract class ViewModel<TOwner> : IListable where TOwner : ModelBase, new()
    {
        protected ViewModel()
        {
            
        }

        protected ViewModel(TOwner owner)
        {
            var fields = GetType().GetProperties().Where(pi => pi.GetCustomAttributes(typeof(MappingAttribute), false).Length > 0);
            foreach (var field in fields)
            {
                var attribute = (MappingAttribute)field.GetCustomAttributes(typeof(MappingAttribute), false).First();

                var propertyValue = owner.GetPropertyValue(attribute.PropertyTree);
                field.SetValue(this, propertyValue, null);
            }
        }

        [Mapping("Id", "Id")]
        public int Id { get; set; }

        public virtual TOwner Update(ISession session)
        {
            var item = session.Get<TOwner>(Id) ?? new TOwner();

            var fields = GetType().GetProperties().Where(pi => pi.GetCustomAttributes(typeof(MappingAttribute), false).Length > 0);
            foreach (var field in fields)
            {
                var attribute = (MappingAttribute)field.GetCustomAttributes(typeof(MappingAttribute), false).First();
                if (attribute.Update)
                {
                    var propertyValue = field.GetValue(this, null);
                    ReflectionHelper.SetPropertyValue(item, attribute.PropertyTree, propertyValue);
                }
            }

            return item;
        }

        public abstract string Readable { get; }

        public List<Input> Inputs(ISession session)
        {
            var fields = GetType().GetProperties().Where(pi => pi.GetCustomAttributes(typeof(BaseInputAttribute), false).Length > 0);

            var inputs = new List<Input>();

            foreach (var field in fields)
            {
                var attribute = (BaseInputAttribute)field.GetCustomAttributes(typeof(BaseInputAttribute), false).First();
                var mappingAttribute = (MappingAttribute)field.GetCustomAttributes(typeof(MappingAttribute), false).First();
                var value = field.GetValue(this, new object[0]);
                var inputHtml = attribute.Render(field.Name, value, mappingAttribute.Name, session);
                var input = new Input
                {
                    order = attribute.Order,
                    label = mappingAttribute.Name,
                    input = inputHtml,
                    size = attribute.Size,
                    smallSize = attribute.SmallSize,
                    labelSize = attribute.LabelSize,
                    labelSmallSize = attribute.LabelSmallSize,
                    showLabel = attribute.ShowLabel
                };

                inputs.Add(input);
            }
            return inputs.OrderBy(x => x.order).ToList();
        }

        public Dictionary<string, string> Errors()
        {
            var fields = GetType().GetProperties().Where(pi => pi.GetCustomAttributes(typeof(BaseInputAttribute), false).Length > 0);

            var errors = new Dictionary<string, string>();

            foreach (var field in fields)
            {
                var attribute = (BaseInputAttribute)field.GetCustomAttributes(typeof(BaseInputAttribute), false).First();
                var mappingAttribute = (MappingAttribute)field.GetCustomAttributes(typeof(MappingAttribute), false).First();
                var value = field.GetValue(this, new object[0]);

                if (!attribute.Isvalid(value)) 
                    errors.Add(mappingAttribute.Name, attribute.GetMessage(mappingAttribute.Name));
            }
            return errors;
        }

        public bool IsValid()
        {
            return Errors().Count == 0;
        }

    }

    public class Input
    {
        public int order;
        public string label;
        public string input;

        public int size;
        public int smallSize;
        public int labelSize;
        public int labelSmallSize;
        public bool showLabel;
    }

}