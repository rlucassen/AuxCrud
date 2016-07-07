﻿namespace AuxCrud.ViewModel.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class LambdaHelper
    {
        public static LambdaExpression GenerateSelector<T>(string propertyName, out Type resultType) where T : class
        {
            // Create a parameter to pass into the Lambda expression (Entity => Entity.OrderByField).
            var parameter = Expression.Parameter(typeof(T), "Entity");
            //  create the selector part, but support child properties
            PropertyInfo property;
            Expression propertyAccess;
            if (propertyName.Contains('.'))
            {
                // support to be sorted on child fields.
                var childProperties = propertyName.Split('.');
                property = typeof(T).GetProperty(childProperties[0]);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
                for (var i = 1; i < childProperties.Length; i++)
                {
                    var tempProp = property;
                    property = tempProp.PropertyType.GetProperty(childProperties[i]) ??
                               tempProp.PropertyType.GetInterfaces().Select(x => x.GetProperty(childProperties[i])).
                                                                                            FirstOrDefault(x => x != null);
                    propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                }
            }
            else
            {
                property = typeof(T).GetProperty(propertyName);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
            }
            resultType = property.PropertyType;
            return Expression.Lambda(propertyAccess, parameter);
        }

        public static Expression<Func<T,bool>> GenerateWhere<T>(string[] propertyNames, string query) where T : class
        {
            var parameter = Expression.Parameter(typeof (T), "Entity");

            var expressions = new List<Expression>();

            foreach (var propertyName in propertyNames)
            {
                expressions.Add(GetContainsExpression<T>(propertyName, query, parameter));
            }

            if (expressions.Count == 1)
            {
                return Expression.Lambda<Func<T, bool>>(expressions[0], parameter);
            }

            var orExpression = expressions[0];
            for (var index = 1; index < expressions.Count; index++)
            {
                orExpression = Expression.OrElse(orExpression, expressions[index]);
            }
            return Expression.Lambda<Func<T, bool>>(orExpression, parameter);

        }

        static Expression GetContainsExpression<T>(string propertyName, string propertyValue, ParameterExpression parameterExp)
        {
            PropertyInfo property;
            Expression propertyAccess;
            if (propertyName.Contains('.'))
            {
                // support to be sorted on child fields.
                var childProperties = propertyName.Split('.');
                property = typeof(T).GetProperty(childProperties[0]);
                propertyAccess = Expression.MakeMemberAccess(parameterExp, property);
                for (var i = 1; i < childProperties.Length; i++)
                {
                    var tempProp = property;
                    property = tempProp.PropertyType.GetProperty(childProperties[i]) ??
                               tempProp.PropertyType.GetInterfaces().Select(x => x.GetProperty(childProperties[i])).
                                                                                            FirstOrDefault(x => x != null);
                    propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                }
            }
            else
            {
                property = typeof(T).GetProperty(propertyName);
                propertyAccess = Expression.MakeMemberAccess(parameterExp, property);
            }

            MethodInfo method = property.PropertyType.GetMethod("Contains", new[] { typeof(string) });
            var someValue = Expression.Constant(propertyValue, typeof(string));
            return Expression.Call(propertyAccess, method, someValue);
        }
    }
}