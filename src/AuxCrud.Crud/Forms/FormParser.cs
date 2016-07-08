namespace AuxCrud.ViewModel.Forms
{
    using System.Reflection;
    using System.Text;
    using Helpers;
    using Model;
    using NHibernate;
    using ViewModel;

    public static class FormParser
    {
        public static string Parse<TOwner, TViewModel>(FormComponent<TOwner, TViewModel> form, ISession session) where TOwner : ModelBase, new() where TViewModel : ViewModel<TOwner, TViewModel>
        {
            if (form == null) return "<p>No form defined</p>";

            var sb = new StringBuilder();
            foreach (var row in form.Rows)
            {
                sb.AppendLine("<div class=\"row\">");

                var label = !string.IsNullOrEmpty(row.GetLabel()) ? row.GetLabel() : "&nbsp;";
                sb.AppendLine($"<div class=\"large-2 columns\"><label>{label}</label></div>");

                var rowSplits = row.GetRowSplits();
                var inputCount = 0;
                foreach (var input in row.Inputs)
                {
                    var propInfo = LambdaHelper.GetMemberExpression(input.Expression).Member as PropertyInfo;
                    var value = propInfo.GetValue(input.ViewModel, null);
                    sb.AppendLine($"<div class=\"large-{rowSplits[inputCount]} columns\">{input.InputField.Render(propInfo.Name, value, LocalizationHelper.String(propInfo), session)}</div>");
                    inputCount++;
                }

                sb.AppendLine($"</div>");
            }


            return sb.ToString();
        }
    }
}