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
            this.AccessorStrategy = new LambdaPropertyAccessorStrategy(conversionEnabled);
            this.IncrementorStrategy = new LambdaPropertyIncrementorStrategy();
        }

        public IPropertyAccessorStrategy AccessorStrategy { get; private set; }

        public IPropertyIncrementorStrategy IncrementorStrategy { get; private set; }

        public IPropertyAccessor<T, TValue> Create<T, TValue>(Expression expression, ITypeConfig type)
        {
            var property = expression.GetLambdaProperty<T>();
            return this.Create<T, TValue>(property, type);
        }

        public IPropertyAccessor<T, TValue> Create<T, TValue>(PropertyInfo property, ITypeConfig type)
        {
            if (property.GetGetMethod() == null || property.GetSetMethod() == null)
            {
                throw new NotImplementedException();
            }
            var get = this.AccessorStrategy.CreateGetter<T, TValue>(property);
            var set = this.AccessorStrategy.CreateSetter<T, TValue>(property);
            var increment = default(Action<T>);
            if (property.PropertyType.IsPrimitive)
            {
                switch (Type.GetTypeCode(property.PropertyType))
                {
                    case TypeCode.Byte:
                    case TypeCode.Single:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                        increment = this.IncrementorStrategy.CreateNumericIncrementor<T>(property);
                        break;
                }
            }
            //We can create binary incrementor only for byte[size] where size is <= sizeof(long).
            else if (property.PropertyType.IsArray && typeof(byte).IsAssignableFrom(property.PropertyType.GetElementType()) && type.Size <= sizeof(long))
            {
                increment = this.IncrementorStrategy.CreateBinaryIncrementor<T>(property, type.Size);
            }
            return new PropertyAccessor<T, TValue>(property, get, set, increment);
        }

        private class PropertyAccessor<T, TValue> : IPropertyAccessor<T, TValue>
        {
            public PropertyAccessor(PropertyInfo property, Func<T, TValue> get, Action<T, TValue> set, Action<T> increment)
            {
                this.Property = property;
                this.Get = get;
                this.Set = set;
                this.Increment = increment;
            }

            public PropertyInfo Property { get; private set; }

            public Func<T, TValue> Get { get; private set; }

            public Action<T, TValue> Set { get; private set; }

            public Action<T> Increment { get; private set; }
        }
    }
}
