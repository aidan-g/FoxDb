using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSetQuery : IEnumerable, IQueryable, IOrderedQueryable, IQueryProvider
    {
        IDatabaseSet Set { get; }

        void Reset();
    }

    public interface IDatabaseSetQuery<T> : IDatabaseSetQuery, IOrderedQueryable<T>, IQueryable<T>, IEnumerable<T>
    {

    }
}
