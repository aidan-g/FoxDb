using FoxDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FoxDb
{
    public class EqualityComparerFactory : IEqualityComparerFactory
    {
        public EqualityComparerFactory()
        {
            this.Members = new DynamicMethod<EqualityComparerFactory>();
            this.EqualityComparers = new ConcurrentDictionary<Type, IEqualityComparer<object>>();
        }

        protected DynamicMethod<EqualityComparerFactory> Members { get; private set; }

        protected ConcurrentDictionary<Type, IEqualityComparer<object>> EqualityComparers { get; private set; }

        public IEqualityComparer<object> Create<T>()
        {
            return this.EqualityComparers.GetOrAdd(typeof(T), key => this.OnCreate<T>());
        }

        public IEqualityComparer<object> Create(Type type)
        {
            return this.EqualityComparers.GetOrAdd(type, key => (IEqualityComparer<object>)this.Members.Invoke(this, "OnCreate", key));
        }

        protected virtual IEqualityComparer<object> OnCreate<T>()
        {
            return new EqualityComparer<T>();
        }

        public static readonly IEqualityComparerFactory Instance = new EqualityComparerFactory();

        public class EqualityComparer<T> : IEqualityComparer<object>
        {
            public EqualityComparer()
            {
                this.InnerEqualityComparer = global::System.Collections.Generic.EqualityComparer<T>.Default;
            }

            public IEqualityComparer<T> InnerEqualityComparer { get; private set; }

            public new bool Equals(object x, object y)
            {
                return this.InnerEqualityComparer.Equals(
                    Converter.ChangeType<T>(x),
                    Converter.ChangeType<T>(y)
                );
            }

            public int GetHashCode(object obj)
            {
                return this.InnerEqualityComparer.GetHashCode(
                    Converter.ChangeType<T>(obj)
                );
            }
        }
    }
}
