namespace FoxDb.Interfaces
{
    public interface IEntityConfiguration
    {
        void Configure(IConfig config, ITableConfig table);
    }
}
