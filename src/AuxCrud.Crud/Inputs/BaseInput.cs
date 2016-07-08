namespace AuxCrud.ViewModel.Inputs
{
    using NHibernate;

    public abstract class BaseInput
    {
        protected BaseInput(string label, bool required = false)
        {
            Required = required;
        }

        public bool Required { get; set; }
        public string Message { get; set; }

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