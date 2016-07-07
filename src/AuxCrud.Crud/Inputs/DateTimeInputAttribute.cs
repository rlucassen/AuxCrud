namespace AuxCrud.ViewModel.Inputs
{
    using System;
    using NHibernate;

    [AttributeUsage(AttributeTargets.Property)]
    public class DateTimeInputAttribute : BaseInputAttribute
    {
        public DateTimeInputAttribute(int order) : base(order)
        {
        }

        public DateTimeInputAttribute(int order, bool required) : base(order, required)
        {
        }

        public override string Render(string property, object value, string name, ISession session)
        {
            var dateTimeValue = Convert.ToDateTime(value);
            var requiredAttr = Required ? "required" : "";
            var stringValue = value != null ? dateTimeValue.ToString("dd-MM-yyyy") : "";
            var input = $"<input class=\"datepicker\" type=\"text\" id=\"item_{property}\" name=\"item.{property}\" value=\"{stringValue}\" {requiredAttr}>";
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