using FoxDb.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public class LambdaPropertyAccessorStrategy : IPropertyAccessorStrategy
    {
        public LambdaPropertyAccessorStrategy(bool conversionEnabled)
        {
            this.ConversionEnabled = conversionEnabled;
        }

        public bool ConversionEnabled { get; private set; }

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
                convert = Expression.Convert(
                    this.ChangeType(parameter2, property.PropertyType),
                    property.PropertyType
                );
            }
            else
            {
                convert = this.ChangeType(parameter2, nullable, property.PropertyType);
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

        protected virtual Expression ChangeType(Expression parameter, Type propertyType)
        {
            if (!this.ConversionEnabled)
            {
                return parameter;
            }
            //Convert(Convert.ChangeType(object value, Type propertyType), propertyType))
            return Expression.Call(
                null,
                typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) }),
                Expression.Convert(parameter, typeof(object)),
                Expression.Constant(propertyType)
            );
        }

        protected virtual Expression ChangeType(Expression parameter, Type nullableType, Type propertyType)
        {
            //IIF(((Param_0 == null) Or (Param_0 == DBNull.Value)), 
            //  Convert(null, propertyType), 
            //  Convert(ChangeType(Convert(Param_0, nullableType), TValue), propertyType))
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
                     this.ChangeType(parameter, nullableType),
                    propertyType
                )
            );
        }
    }
}
