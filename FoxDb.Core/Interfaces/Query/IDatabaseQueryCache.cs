using System;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryCache
    {
        IDatabaseQuery Fetch(ITableConfig table);

        IDatabaseQuery Add(ITableConfig table);

        IDatabaseQuery Update(ITableConfig table);

        IDatabaseQuery Delete(ITableConfig table);

        IDatabaseQuery GetOrAdd(IDatabaseQueryCacheKey key, Func<IDatabaseQuery> factory);
    }

    public interface IDatabaseQueryCacheKey : IEquatable<IDatabaseQueryCacheKey>
    {
        string Id { get; }
    }
}
