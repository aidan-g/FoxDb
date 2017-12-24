namespace FoxDb.Interfaces
{
    public interface IConfig
    {
        ITableConfig<T> Table<T>(bool useDefaultColumns = true) where T : IPersistable;

        ITableConfig<T1, T2> Table<T1, T2>(bool useDefaultColumns = true) where T1 : IPersistable where T2 : IPersistable;
    }
}
