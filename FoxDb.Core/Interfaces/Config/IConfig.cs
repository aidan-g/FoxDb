using System;

namespace FoxDb.Interfaces
{
    public interface IConfig
    {
        ITableConfig Table(Type tableType, bool useDefaultColumns = true);

        ITableConfig<T> Table<T>(bool useDefaultColumns = true);

        IIntermediateTableConfig Table(Type parentTableType, Type childTableType, bool useDefaultColumns = true);

        ITableConfig<T1, T2> Table<T1, T2>(bool useDefaultColumns = true);
    }
}
