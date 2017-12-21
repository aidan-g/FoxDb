namespace FoxDb.Interfaces
{
    public interface IConfig
    {
        ITableConfig<T> Table<T>() where T : IPersistable;
    }
}
