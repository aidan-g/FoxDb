using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSetBase : IDatabaseQuerySource
    {
        ITableConfig Table { get; }

        Type ElementType { get; }
    }

    public partial interface IDatabaseSet : IDatabaseSetBase, ICollection
    {
        object Create();

        object Find(params object[] keys);

        object AddOrUpdate(object item);

        IEnumerable<object> AddOrUpdate(IEnumerable<object> items);

        IEnumerable<object> Remove(IEnumerable<object> items);
    }

    public partial interface IDatabaseSet : IAsyncEnumerable
    {
        Task<int> CountAsync { get; }

        Task<bool> ContainsAsync(object item);

        Task<object> FindAsync(params object[] keys);

        Task AddAsync(object item);

        Task<object> AddOrUpdateAsync(object item);

        Task<IEnumerable<object>> AddOrUpdateAsync(IEnumerable<object> items);

        Task<bool> RemoveAsync(object item);

        Task<IEnumerable<object>> RemoveAsync(IEnumerable<object> items);

        Task ClearAsync();
    }

    public partial interface IDatabaseSet<T> : IDatabaseSetBase, ICollection<T>
    {
        T Create();

        T Find(params object[] keys);

        T AddOrUpdate(T item);

        IEnumerable<T> AddOrUpdate(IEnumerable<T> items);

        IEnumerable<T> Remove(IEnumerable<T> items);
    }

    public partial interface IDatabaseSet<T> : IAsyncEnumerable<T>
    {
        Task<int> CountAsync { get; }

        Task<bool> ContainsAsync(T item);

        Task<T> FindAsync(params object[] keys);

        Task AddAsync(T item);

        Task<T> AddOrUpdateAsync(T item);

        Task<IEnumerable<T>> AddOrUpdateAsync(IEnumerable<T> items);

        Task<bool> RemoveAsync(T item);

        Task<IEnumerable<T>> RemoveAsync(IEnumerable<T> items);

        Task ClearAsync();
    }
}
