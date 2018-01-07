namespace FoxDb.Interfaces
{
    public interface ITableFactory
    {
        ITableConfig<T> Create<T>(IConfig config);

        ITableConfig<T1, T2> Create<T1, T2>(IConfig config);
    }
}
