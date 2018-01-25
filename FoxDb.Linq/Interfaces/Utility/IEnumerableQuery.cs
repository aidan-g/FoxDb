using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb.Interfaces
{
    public interface IEnumerableQuery : IEnumerable, IQueryable, IOrderedQueryable
    {

    }

    public interface IEnumerableQuery<T> : IEnumerableQuery, IOrderedQueryable<T>, IQueryable<T>, IEnumerable<T>
    {
    }
}
