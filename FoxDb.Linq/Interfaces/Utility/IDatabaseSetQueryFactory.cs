using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSetQueryFactory : IEnumerable, IQueryable, IOrderedQueryable, IQueryProvider
    {
        IDatabase Database { get; }

        ITransactionSource Transaction { get; }
    }

    public interface IDatabaseSetQueryFactory<T> : IDatabaseSetQueryFactory, IOrderedQueryable<T>, IQueryable<T>, IEnumerable<T>
    {

    }
}
