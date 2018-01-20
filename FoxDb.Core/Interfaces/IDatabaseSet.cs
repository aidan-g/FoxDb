using System;
using System.Collections;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSet : IDatabaseQuerySource, IEnumerable
    {
        Type ElementType { get; }
    }

    public interface IDatabaseSet<T> : IDatabaseSet, ICollection<T>
    {
        T Create();

        T Find(object id);

        T AddOrUpdate(T item);

        IEnumerable<T> AddOrUpdate(IEnumerable<T> items);

        IEnumerable<T> Remove(IEnumerable<T> items);
    }
}
