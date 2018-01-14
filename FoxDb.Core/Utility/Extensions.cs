using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static T CreateDelegate<T>(this MethodInfo method)
        {
            return (T)(object)method.CreateDelegate(typeof(T));
        }

        public static PropertyInfo GetLambdaProperty<T>(this Expression expression)
        {
            if (expression.NodeType != ExpressionType.Lambda)
            {
                throw new NotImplementedException();
            }
            var lambda = expression as LambdaExpression;
            if (lambda.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new NotImplementedException();
            }
            var member = lambda.Body as MemberExpression;
            if (!(member.Member is PropertyInfo))
            {
                throw new NotImplementedException();
            }
            var property = member.Member as PropertyInfo;
            if (property.DeclaringType == typeof(T))
            {
                return property;
            }
            return typeof(T).GetProperty(property.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly) ?? typeof(T).GetProperty(property.Name);
        }

    }
}
