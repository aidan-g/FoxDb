namespace FoxDb.Interfaces
{
    public interface IIndexFactory
    {
        IIndexConfig Create(ITableConfig table, IIndexSelector selector);
    }
}
