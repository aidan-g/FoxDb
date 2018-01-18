namespace FoxDb.Interfaces
{
    public interface IEntityFactory
    {
        ITableConfig Table { get; }

        IEntityInitializer Initializer { get; }

        IEntityPopulator Populator { get; }

        object Create();

        object Create(IDatabaseReaderRecord record);
    }
}
