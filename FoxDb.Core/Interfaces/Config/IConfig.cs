namespace FoxDb.Interfaces
{
    public interface IConfig
    {
        ITableConfig<T> Table<T>() where T : IPersistable;

        ITableConfig<T1, T2> Table<T1, T2>() where T1 : IPersistable where T2 : IPersistable;
    }
}
