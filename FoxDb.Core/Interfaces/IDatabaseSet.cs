using System.Collections.Generic;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSet<T> : System.Collections.Generic.IEnumerable<T> where T : IPersistable
    {
        IDatabase Database { get; }

        IDatabaseQuerySource<T> Source { get; }

        DatabaseParameterHandler Parameters { get; }

        IDbTransaction Transaction { get; }

        int Count { get; }

        T Find(object id);

        T AddOrUpdate(T item);

        IEnumerable<T> AddOrUpdate(System.Collections.Generic.IEnumerable<T> items);

        T Delete(T item);

        IEnumerable<T> Delete(System.Collections.Generic.IEnumerable<T> items);

        void Clear();
    }
}
