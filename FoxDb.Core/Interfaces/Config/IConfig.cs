namespace FoxDb.Interfaces
{
    public interface IConfig
    {
        IDatabase Database { get; }

        ITableConfig GetTable(ITableSelector selector);

        ITableConfig CreateTable(ITableSelector selector);

        bool TryCreateTable(ITableSelector selector, out ITableConfig table);
    }
}
