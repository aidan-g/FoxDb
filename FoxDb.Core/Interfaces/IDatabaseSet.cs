using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSet : IEnumerable
    {
        IDatabase Database { get; }

        DatabaseParameterHandler Parameters { get; }

        IDbTransaction Transaction { get; }

        int Count { get; }

        void Clear();
    }

    public interface IDatabaseSet<T> : IDatabaseSet, IEnumerable<T>
    {
        IDatabaseQuerySource<T> Source { get; }

        T Find(object id);

        T AddOrUpdate(T item);

        IEnumerable<T> AddOrUpdate(IEnumerable<T> items);

        T Delete(T item);

        IEnumerable<T> Delete(IEnumerable<T> items);

    }
}
