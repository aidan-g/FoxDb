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

        void AddOrUpdate(T item);

        void AddOrUpdate(IEnumerable<T> items);

        void Delete(T item);

        void Delete(IEnumerable<T> items);

        void Clear();
    }
}
