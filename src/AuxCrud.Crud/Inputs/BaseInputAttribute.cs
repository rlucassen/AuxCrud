namespace AuxCrud.ViewModel.Inputs
{
    using System;
    using NHibernate;

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class BaseInputAttribute : Attribute
    {
        protected BaseInputAttribute(int order)
        {
            Order = order;
        }

        protected BaseInputAttribute(int order, bool required) : this(order)
        {
            Required = required;
        }

        public int Order { get; set; }
        public bool Required { get; set; }
        public string Message { get; set; }

        public int Size { get; set; } = 10;
        public int SmallSize { get; set; } = 8;
        public int LabelSize { get; set; } = 2;
        public int LabelSmallSize { get; set; } = 4;
        public bool ShowLabel { get; set; } = true;

        public abstract string Render(string property, object value, string name, ISession session);
        public abstract bool Isvalid(object value);

        public string GetMessage(string name)
        {
            return string.IsNullOrEmpty(Message) ? $"{name} is verplicht" : Message;
        }
    }
}