namespace FoxDb.Interfaces
{
    public interface IEntityPopulator
    {
        ITableConfig Table { get; }

        void Populate(object item, IDatabaseReaderRecord record);
    }
}
