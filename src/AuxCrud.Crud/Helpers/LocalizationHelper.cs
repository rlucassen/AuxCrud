namespace AuxCrud.ViewModel.Helpers
{
    using System.Reflection;

    public static class LocalizationHelper
    {
        public static string String(PropertyInfo propInfo)
        {
            if (propInfo.DeclaringType == null) return "";
            var entityName = propInfo.DeclaringType.Name.Replace("ViewModel", "");
            var s = Localization.Language.ResourceManager.GetString($"{entityName}_{propInfo.Name}");
            return !string.IsNullOrEmpty(s) ? s : propInfo.Name;
        }
    }
}