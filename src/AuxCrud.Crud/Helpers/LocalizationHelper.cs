namespace AuxCrud.ViewModel.Helpers
{
    using System;
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

        public static string String(Type type)
        {
            var entityName = type.Name.Replace("ViewModel", "");
            return Localization.Language.ResourceManager.GetString($"{entityName}");
        }

        public static string String(Type type, string suffix)
        {
            var entityName = type.Name.Replace("ViewModel", "");
            return Localization.Language.ResourceManager.GetString($"{entityName}{suffix}");
        }
    }
}