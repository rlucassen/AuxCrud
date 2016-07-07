namespace AuxCrud.ViewModel.Helpers
{
    using System;
    using System.Reflection;

    public static class ReflectionHelper
    {
        public static bool PropertyTreeExists(this Type type, string propertyTree)
        {
            foreach(var prop in propertyTree.Split('.'))
            {
                var propertyInfo = type.GetProperty(prop);
                if (propertyInfo == null) return false;
                type = propertyInfo.GetType();
            }
            return true;
        }

        public static PropertyInfo GetPropertyInfo(this Type type, string property)
        {
            PropertyInfo propertyInfo = null;
            foreach (var propName in property.Split('.'))
            {
                propertyInfo = type.GetProperty(propName);
                type = propertyInfo.GetType();
            }
            return propertyInfo;
        }

        public static object GetPropertyValue(this object obj, string propertyTree)
        {
            var propNames = propertyTree.Split('.');
            foreach (var propName in propNames)
            {
                if (obj == null) return null;
                var propertyInfo = obj.GetType().GetProperty(propName);
                obj = propertyInfo.GetValue(obj, null);
            }

            return obj;
        }

        public static void SetPropertyValue(this object obj, string propertyTree, object value)
        {
            var propNames = propertyTree.Split('.');
            PropertyInfo propertyInfo = null;

            object tempObject = obj;
            foreach (var propName in propNames)
            {
                if (tempObject == null) return;
                propertyInfo = tempObject.GetType().GetProperty(propName);
                tempObject = propertyInfo.GetValue(obj, null);
            }

            propertyInfo?.SetValue(obj, value, null);
        }
    }
}