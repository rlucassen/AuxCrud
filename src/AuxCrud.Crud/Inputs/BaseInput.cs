namespace AuxCrud.ViewModel.Inputs
{
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

        public static class Patterns
        {
            public const string Email = @"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)+$";
            public const string Postcode = @"^[0-9]{4}[a-zA-Z]{2}$";
            public const string Phone = @"^[0-9]{10}$";
            public const string NoZero = @"^[1-9]+[0-9]*$";
        }

    }
}