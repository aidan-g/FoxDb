using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSet<T> : IEnumerable<T> where T : IPersistable
    {
        IDatabase Database { get; }

        int Count { get; }

        T Find(object id);

        void AddOrUpdate(T item);

        void AddOrUpdate(IEnumerable<T> items);

        void Delete(T item);

        void Delete(IEnumerable<T> items);

        void Clear();
    }
}
