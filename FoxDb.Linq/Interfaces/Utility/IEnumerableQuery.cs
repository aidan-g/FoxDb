using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb.Interfaces
{
    public interface IEnumerableQuery : ICollection, IQueryable, IOrderedQueryable
    {

    }

    public interface IEnumerableQuery<T> : IEnumerableQuery, ICollection<T>, IOrderedQueryable<T>, IQueryable<T>, IAsyncEnumerable<T>
    {
    }
}
