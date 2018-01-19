using FoxDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace FoxDb
{
    public class FastActivator : IFastActivator
    {
        public FastActivator()
        {
            this.Constructors = new ConcurrentDictionary<Type, Func<object>>();
        }

        protected ConcurrentDictionary<Type, Func<object>> Constructors { get; private set; }

        public object Activate(Type type)
        {
            var constructor = default(Func<object>);
            if (!this.Constructors.TryGetValue(type, out constructor))
            {
                constructor = Expression.Lambda<Func<object>>(Expression.Convert(Expression.New(type), typeof(object))).Compile();
                this.Constructors.TryAdd(type, constructor);
            }
            return constructor();
        }

        public static readonly IFastActivator Instance = new FastActivator();
    }

    public class FastActivator<T> : IFastActivator<T>
    {
        protected static readonly Func<T> Constructor = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();

        public T Activate()
        {
            return Constructor();
        }

        public static readonly IFastActivator<T> Instance = new FastActivator<T>();
    }
}
