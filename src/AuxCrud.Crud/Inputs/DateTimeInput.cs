namespace AuxCrud.ViewModel.Inputs
{
    using System;
    using NHibernate;

    public class DateTimeInput : BaseInput
    {
        public DateTimeInput(string label, bool required = false) : base(label, required)
        {
        }

        public override string Render(string property, object value, string name, ISession session)
        {
            var dateTimeValue = Convert.ToDateTime(value);
            var requiredAttr = Required ? "required" : "";
            var stringValue = value != null ? dateTimeValue.ToString("dd-MM-yyyy") : "";
            var input = $"<input class=\"datepicker\" type=\"text\" id=\"item_{property}\" name=\"item.{property}\" value=\"{stringValue}\" placeholder=\"{name}\" {requiredAttr}>";
            var messageElem = Required ? $"<small class=\"error\">{GetMessage(name)}</small>" : "";
            return $"{input}{messageElem}";
        }

        public override bool Isvalid(object value)
        {
            var dateTimeValue = Convert.ToDateTime(value);
            return !Required || dateTimeValue.Year > 1753;
        }
    }
}