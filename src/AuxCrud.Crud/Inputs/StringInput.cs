namespace AuxCrud.ViewModel.Inputs
{
    using System;
    using NHibernate;

    public class StringInput : BaseInput
    {
        public StringInput()
        {
        }

        public StringInput(bool required)
        {
            Required = required;
        }


        public string Pattern { get; set; }

        public override string Render(string property, object value, string name, ISession session)
        {
            var patternAttr = !string.IsNullOrEmpty(Pattern) ? $"pattern=\"{Pattern}\"" : "";
            var requiredAttr = Required ? "required" : "";
            var input = $"<input type=\"text\" id=\"item_{property}\" name=\"item.{property}\" value=\"{value}\" placeholder=\"\" {patternAttr} {requiredAttr}>";
            var messageElem = !string.IsNullOrEmpty(Pattern) || Required ? $"<small class=\"error\">{GetMessage(name)}</small>" : "";
            return $"{input}{messageElem}";
        }

        public override bool Isvalid(object value)
        {
            return !Required || !string.IsNullOrEmpty((string) value);
        }

        public static class Patterns
        {
            public const string Email = @"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)+$";
            public const string Postcode = @"^[0-9]{4}[a-zA-Z]{2}$";
            public const string Phone = @"^[0-9]{10}$";
        }
    }
}