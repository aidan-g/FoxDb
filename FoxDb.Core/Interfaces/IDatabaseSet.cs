using System.Collections.Generic;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSet<T> : IEnumerable<T> where T : IPersistable
    {
        IDatabase Database { get; }

        IDatabaseQuerySource<T> Source { get; }

        DatabaseParameterHandler Parameters { get; }

        IDbTransaction Transaction { get; }

        int Count { get; }

        T Find(object id);

        T AddOrUpdate(T item);

        IEnumerable<T> AddOrUpdate(IEnumerable<T> items);

        T Delete(T item);

        IEnumerable<T> Delete(IEnumerable<T> items);

        void Clear();
    }
}
