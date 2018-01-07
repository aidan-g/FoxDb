using System;

namespace FoxDb.Interfaces
{
    public interface IConfig
    {
        ITableConfig Table(Type tableType, ConfigDefaults defaults = ConfigDefaults.Default);

        ITableConfig<T> Table<T>(ConfigDefaults defaults = ConfigDefaults.Default);

        ITableConfig<T1, T2> Table<T1, T2>(ConfigDefaults defaults = ConfigDefaults.Default);
    }

    [Flags]
    public enum ConfigDefaults : byte
    {
        None = 0,
        DefaultColumns = 1,
        DefaultRelations = 2,
        Default = DefaultColumns | DefaultRelations
    }
}
