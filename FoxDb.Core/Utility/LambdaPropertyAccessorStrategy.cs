using FoxDb.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public class LambdaPropertyAccessorStrategy : IPropertyAccessorStrategy
    {
        public Func<T, TValue> CreateGetter<T, TValue>(PropertyInfo property)
        {
            var parameter = Expression.Parameter(typeof(T));
            return Expression.Lambda<Func<T, TValue>>(
                Expression.Convert(
                    Expression.Property(
                        Expression.Convert(parameter, property.DeclaringType),
                        property
                    ),
                    typeof(TValue)
                ),
                parameter
            ).Compile();
        }

        public Action<T, TValue> CreateSetter<T, TValue>(PropertyInfo property)
        {
            var parameter1 = Expression.Parameter(typeof(T));
            var parameter2 = Expression.Parameter(typeof(TValue));
            return Expression.Lambda<Action<T, TValue>>(
                Expression.Assign(
                    Expression.Property(
                        Expression.Convert(parameter1, property.DeclaringType),
                        property
                    ),
                    Expression.Convert(parameter2, property.PropertyType)
                ),
                parameter1,
                parameter2
            ).Compile();
        }
    }
}
