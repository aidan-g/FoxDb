using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSet : IEnumerable
    {
        Type ElementType { get; }

        IDatabase Database { get; }

        ITableConfig Table { get; }

        IEntityMapper Mapper { get; }

        IEntityInitializer Initializer { get; }

        IEntityPopulator Populator { get; }

        IEntityFactory Factory { get; }

        IDatabaseQuerySource Source { get; }

        IDbTransaction Transaction { get; }
    }

    public interface IDatabaseSet<T> : IDatabaseSet, ICollection<T>
    {
        T Create();

        T Find(object id);

        T AddOrUpdate(T item);

        IEnumerable<T> AddOrUpdate(IEnumerable<T> items);

        T Delete(T item);

        IEnumerable<T> Delete(IEnumerable<T> items);
    }
}
