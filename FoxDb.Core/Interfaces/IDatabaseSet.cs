using System;
using System.Collections;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSet : IDatabaseQuerySource, IEnumerable
    {
        ITableConfig Table { get; }

        Type ElementType { get; }

        object Create();

        object Find(params object[] keys);

        object AddOrUpdate(object item);

        IEnumerable<object> AddOrUpdate(IEnumerable<object> items);

        IEnumerable<object> Remove(IEnumerable<object> items);
    }

    public interface IDatabaseSet<T> : IDatabaseSet, ICollection<T>
    {
        new T Create();

        new T Find(params object[] keys);

        T AddOrUpdate(T item);

        IEnumerable<T> AddOrUpdate(IEnumerable<T> items);

        IEnumerable<T> Remove(IEnumerable<T> items);
    }
}
