using FoxDb.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public class LambdaPropertyIncrementorStrategy : IPropertyIncrementorStrategy
    {
        public Action<T> CreateNumericIncrementor<T>(PropertyInfo property)
        {
            var parameter = Expression.Parameter(typeof(T));
            var lambda = Expression.Lambda<Action<T>>(
                Expression.Assign(
                    Expression.Property(
                        Expression.Convert(parameter, property.DeclaringType),
                        property
                    ),
                    Expression.MakeBinary(
                        ExpressionType.Add,
                        Expression.Property(
                            Expression.Convert(parameter, property.DeclaringType),
                            property
                        ),
                        Expression.Convert(
                            Expression.Constant(1),
                            property.PropertyType
                        )
                    )
                ),
                parameter
            );
            return lambda.Compile();
        }

        public Action<T> CreateBinaryIncrementor<T>(PropertyInfo property, int capacity)
        {
            var parameter = Expression.Parameter(typeof(T));
            var temp = Expression.Parameter(typeof(byte[]));
            var lambda = Expression.Lambda<Action<T>>(
                Expression.Block(
                    new[] 
                    {
                        temp
                    },
                    Expression.Assign(
                        temp,
                        Expression.NewArrayBounds(
                            typeof(byte),
                            Expression.Constant(capacity)
                        )
                    ),
                    Expression.Call(
                        typeof(Array).GetMethod("Copy", new[] { typeof(Array), typeof(Array), typeof(int) }),
                        Expression.Property(
                            Expression.Convert(parameter, property.DeclaringType),
                            property
                        ),
                        temp,
                        Expression.Constant(capacity)
                    ),
                    Expression.Call(
                        typeof(Array).GetMethod("Reverse", new[] { typeof(Array) }),
                        temp
                    ),
                    Expression.Assign(
                        temp,
                        Expression.Call(
                            typeof(BitConverter).GetMethod("GetBytes", new[] { this.GetRepresentationType(capacity) }),
                            Expression.MakeBinary(
                                ExpressionType.Add,
                                Expression.Call(
                                    typeof(BitConverter).GetMethod(
                                        this.GetRepresentationMethod(capacity)
                                    ),
                                    temp,
                                    Expression.Constant(0)
                                ),
                                Expression.Convert(
                                    Expression.Constant(1),
                                    this.GetRepresentationType(capacity)
                                )
                            )
                        )
                    ),
                    Expression.Call(
                        typeof(Array).GetMethod("Reverse", new[] { typeof(Array) }),
                        temp
                    ),
                    Expression.Assign(
                        Expression.Property(
                            Expression.Convert(parameter, property.DeclaringType),
                            property
                        ),
                        temp
                    )
                ),
                parameter
            );
            return lambda.Compile();
        }

        protected virtual Type GetRepresentationType(int capacity)
        {
            switch (capacity)
            {
                case sizeof(short):
                    return typeof(short);
                case sizeof(int):
                    return typeof(int);
                case sizeof(long):
                    return typeof(long);
            }
            throw new NotImplementedException();
        }

        protected virtual string GetRepresentationMethod(int capacity)
        {
            switch (capacity)
            {
                case sizeof(short):
                    return "ToInt16";
                case sizeof(int):
                    return "ToInt32";
                case sizeof(long):
                    return "ToInt64";
            }
            throw new NotImplementedException();
        }
    }
}
