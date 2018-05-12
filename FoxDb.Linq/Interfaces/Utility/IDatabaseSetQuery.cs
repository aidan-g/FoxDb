using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSetQuery : IEnumerable, IQueryable, IOrderedQueryable, IQueryProvider
    {

    }

    public interface IDatabaseSetQuery<T> : IDatabaseSetQuery, IOrderedQueryable<T>, IQueryable<T>, IEnumerable<T>
    {

    }
}
