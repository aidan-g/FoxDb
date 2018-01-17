using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSetEnumerableQuery : IEnumerable, IQueryable, IOrderedQueryable, IQueryProvider
    {

    }

    public interface IDatabaseSetEnumerableQuery<T> : IDatabaseSetEnumerableQuery, IOrderedQueryable<T>, IQueryable<T>, IEnumerable<T>
    {
        IDatabaseSet<T> Set { get; }
    }
}
