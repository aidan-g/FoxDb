namespace FoxDb.Interfaces
{
    public interface IConfig
    {
        ITableConfig<T> Table<T>(bool useDefaultColumns = true);

        ITableConfig<T1, T2> Table<T1, T2>(bool useDefaultColumns = true);
    }
}
