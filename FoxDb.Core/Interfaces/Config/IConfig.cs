using System;

namespace FoxDb.Interfaces
{
    public interface IConfig
    {
        IDatabase Database { get; }

        ITableConfig Table(Type tableType);

        ITableConfig<T> Table<T>();

        ITableConfig<T1, T2> Table<T1, T2>();
    }
}
