using FoxDb.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public class PropertyAccessorFactory : IPropertyAccessorFactory
    {
        public PropertyAccessorFactory(bool conversionEnabled)
        {
            this.Strategy = new LambdaPropertyAccessorStrategy(conversionEnabled);
        }

        public IPropertyAccessorStrategy Strategy { get; private set; }

        public Expression Create(PropertyInfo property)
        {
            var parameter = Expression.Parameter(property.DeclaringType);
            return Expression.Lambda(Expression.Property(parameter, property), parameter);
        }

        public IPropertyAccessor<T, TValue> Create<T, TValue>(Expression expression)
        {
            var property = expression.GetLambdaProperty<T>();
            return this.Create<T, TValue>(property);
        }

        public IPropertyAccessor<T, TValue> Create<T, TValue>(PropertyInfo property)
        {
            if (property.GetGetMethod() == null || property.GetSetMethod() == null)
            {
                throw new NotImplementedException();
            }
            var get = this.Strategy.CreateGetter<T, TValue>(property);
            var set = this.Strategy.CreateSetter<T, TValue>(property);
            return new PropertyAccessor<T, TValue>(property, get, set);
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
