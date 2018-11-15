using System;
using System.Collections;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSetBase : IDatabaseQuerySource
    {
        ITableConfig Table { get; }

        Type ElementType { get; }
    }

    public interface IDatabaseSet : IDatabaseSetBase, ICollection
    {
        object Create();

        object Find(params object[] keys);

        object AddOrUpdate(object item);

        IEnumerable<object> AddOrUpdate(IEnumerable<object> items);

        IEnumerable<object> Remove(IEnumerable<object> items);
    }

    public interface IDatabaseSet<T> : IDatabaseSetBase, ICollection<T>
    {
        T Create();

        T Find(params object[] keys);

        T AddOrUpdate(T item);

        IEnumerable<T> AddOrUpdate(IEnumerable<T> items);

        IEnumerable<T> Remove(IEnumerable<T> items);
    }
}
