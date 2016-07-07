namespace AuxCrud.ViewModel.Inputs
{
    using System;
    using NHibernate;

    public abstract class BaseInput
    {
        protected BaseInput(bool required = false)
        {
            Required = required;
        }

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