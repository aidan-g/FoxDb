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
            var nullable = Nullable.GetUnderlyingType(property.PropertyType);
            var parameter1 = Expression.Parameter(typeof(T));
            var parameter2 = Expression.Parameter(typeof(TValue));
            var convert = default(Expression);
            if (nullable == null)
            {
                //Convert(Param_0, PropertyType)
                convert = Expression.Convert(parameter2, property.PropertyType);
            }
            else
            {
                convert = this.ConvertToNullable(parameter2, nullable, property.PropertyType);
            }
            var lambda = Expression.Lambda<Action<T, TValue>>(
                Expression.Assign(
                    Expression.Property(
                        Expression.Convert(parameter1, property.DeclaringType),
                        property
                    ),
                    convert
                ),
                parameter1,
                parameter2
            );
            return lambda.Compile();
        }

        protected virtual Expression ConvertToNullable(Expression parameter, Type nullableType, Type propertyType)
        {
            //IIF(((Param_0 == null) Or (Param_0 == DBNull.Value)), 
            //  Convert(null, PropertyType), 
            //  Convert(ChangeType(Convert(Param_0, NullableType), TValue), PropertyType))
            return Expression.Condition(
                Expression.Or(
                    Expression.Equal(parameter, Expression.Constant(null)),
                    Expression.Equal(parameter, Expression.Constant(DBNull.Value))
                ),
                Expression.Convert(
                    Expression.Constant(null),
                    propertyType
                ),
                Expression.Convert(
                    Expression.Call(
                        null,
                        //Convert.ChangeType(object value, Type conversionType)
                        typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) }),
                        Expression.Convert(parameter, typeof(object)),
                        Expression.Constant(nullableType)
                    ),
                    propertyType
                )
            );
        }
    }
}
