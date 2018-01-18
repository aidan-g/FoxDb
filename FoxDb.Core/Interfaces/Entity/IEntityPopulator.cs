namespace FoxDb.Interfaces
{
    public interface IEntityPopulator
    {
        ITableConfig Table { get; }

        IEntityMapper Mapper { get; }

        void Populate(object item, IDatabaseReaderRecord record);
    }
}
