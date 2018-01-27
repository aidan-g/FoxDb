using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSetQueryFactory : IEnumerable, IQueryable, IOrderedQueryable, IQueryProvider
    {

    }

    public interface IDatabaseSetQueryFactory<T> : IDatabaseSetQueryFactory, IOrderedQueryable<T>, IQueryable<T>, IEnumerable<T>
    {

    }
}
