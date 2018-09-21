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
            var interimType = default(Type);
            var parameter1 = Expression.Parameter(typeof(T));
            var parameter2 = Expression.Parameter(typeof(TValue));
            var convert = default(Expression);
            if (TypeHelper.TryGetInterimType(property.PropertyType, out interimType))
            {
                convert = this.ChangeType(parameter2, interimType, property.PropertyType);
            }
            else
            {
                convert = Expression.Convert(
                    this.ChangeType(parameter2, property.PropertyType),
                    property.PropertyType
                );
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

        protected virtual Expression ChangeType(Expression parameter, Type targetType)
        {
            if (!this.ConversionEnabled)
            {
                return parameter;
            }
            //Convert(Convert.ChangeType(object value, Type propertyType), targetType))
            return Expression.Call(
                null,
                typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) }),
                Expression.Convert(parameter, typeof(object)),
                Expression.Constant(targetType)
            );
        }

        protected virtual Expression ChangeType(Expression parameter, Type interimType, Type targetType)
        {
            //IIF(((Param_0 == null) Or (Param_0 == DBNull.Value)), 
            //  Convert(null, targetType), 
            //  Convert(ChangeType(Convert(Param_0, interimType), TValue), targetType))
            return Expression.Condition(
                Expression.Or(
                    Expression.Equal(parameter, Expression.Constant(null)),
                    Expression.Equal(parameter, Expression.Constant(DBNull.Value))
                ),
                Expression.Convert(
                    Expression.Constant(null),
                    targetType
                ),
                Expression.Convert(
                     this.ChangeType(parameter, interimType),
                    targetType
                )
            );
        }
    }
}
