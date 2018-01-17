namespace FoxDb.Interfaces
{
    public interface ITableFactory
    {
        ITableConfig Create(IConfig config, ITableSelector selector);
    }
}
