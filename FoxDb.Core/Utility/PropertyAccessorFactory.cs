using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public static class PropertyAccessorFactory
    {
        public static Expression Create(PropertyInfo property)
        {
            var parameter = Expression.Parameter(property.DeclaringType);
            return Expression.Lambda(Expression.Property(parameter, property), parameter);
        }

        public static IPropertyAccessor<T, TValue> Create<T, TValue>(Expression expression)
        {
            var property = GetLambdaProperty(expression);
            if (!property.CanRead || !property.CanWrite)
            {
                throw new NotImplementedException();
            }
            var get = CreateGetter<T, TValue>(property);
            var set = CreateSetter<T, TValue>(property);
            return new PropertyAccessor<T, TValue>(property, get, set);
        }

        private static PropertyInfo GetLambdaProperty(Expression expression)
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
            return member.Member as PropertyInfo;
        }

        private static Func<T, TValue> CreateGetter<T, TValue>(PropertyInfo property)
        {
            if (typeof(TValue).IsAssignableFrom(property.PropertyType))
            {
                return property.GetGetMethod().CreateDelegate<Func<T, TValue>>();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static Action<T, TValue> CreateSetter<T, TValue>(PropertyInfo property)
        {
            if (property.PropertyType.IsAssignableFrom(typeof(TValue)))
            {
                return property.GetSetMethod().CreateDelegate<Action<T, TValue>>();
            }
            else
            {
                var parameter1 = Expression.Parameter(typeof(T));
                var parameter2 = Expression.Parameter(typeof(TValue));
                return Expression.Lambda<Action<T, TValue>>(
                    Expression.Assign(
                        Expression.Property(parameter1, property),
                        Expression.Convert(parameter2, property.PropertyType)
                    ),
                    parameter1,
                    parameter2
                ).Compile();
            }
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
