namespace AuxCrud.Model.Helpers
{
    using System.Globalization;
    using System.Threading;

    public static class StringHelper
    {
        public static string Capitalize(this string str)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            return textInfo.ToTitleCase(str);
        }
    }
}