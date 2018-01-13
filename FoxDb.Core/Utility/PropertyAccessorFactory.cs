using FoxDb.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public static class PropertyAccessorFactory
    {
        public static readonly IPropertyAccessorStrategy Strategy = new LambdaPropertyAccessorStrategy();

        public static Expression Create(PropertyInfo property)
        {
            var parameter = Expression.Parameter(property.DeclaringType);
            return Expression.Lambda(Expression.Property(parameter, property), parameter);
        }

        public static IPropertyAccessor<T, TValue> Create<T, TValue>(Expression expression)
        {
            var property = GetLambdaProperty<T>(expression);
            return Create<T, TValue>(property);
        }

        public static IPropertyAccessor<T, TValue> Create<T, TValue>(PropertyInfo property)
        {
            if (property.GetGetMethod() == null || property.GetSetMethod() == null)
            {
                throw new NotImplementedException();
            }
            var get = Strategy.CreateGetter<T, TValue>(property);
            var set = Strategy.CreateSetter<T, TValue>(property);
            return new PropertyAccessor<T, TValue>(property, get, set);
        }

        public static PropertyInfo GetLambdaProperty<T>(Expression expression)
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

        private class PropertyAccessor<T, TValue> : IPropertyAccessor<T, TValue>
        {
            public PropertyAccessor(PropertyInfo property, Func<T, TValue> get, Action<T, TValue> set)
            {
                this.Property = property;
                this.Get = get;
                this.Set = set;
            }

            public PropertyInfo Property { get; private set; }

            public Func<T, TValue> Get { get; private set; }

            public Action<T, TValue> Set { get; private set; }
        }
    }
}
