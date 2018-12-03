namespace FoxDb.Interfaces
{
    public interface IConfig
    {
        IDatabase Database { get; }

        IConfig Transient { get; }

        ITableConfig GetTable(ITableSelector selector);

        ITableConfig CreateTable(ITableSelector selector);

        bool TryCreateTable(ITableSelector selector, out ITableConfig table);

        void Reset();

        void CopyTo(IConfig config);
    }
}
