namespace AuxCrud.ViewModel.Inputs
{
    using System.Text.RegularExpressions;
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
            var stringValue = (string) value ?? "";
            if (!string.IsNullOrEmpty(Pattern))
            {
                var regex = new Regex(Pattern);
                return regex.IsMatch(stringValue);
            }
            return !Required || !string.IsNullOrEmpty(stringValue);
        }

    }
}