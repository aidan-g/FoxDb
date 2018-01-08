namespace FoxDb.Interfaces
{
    public interface IEntityPopulatorStrategy
    {
        bool Populate(object item, IColumnConfig column, IDatabaseReaderRecord record);
    }
}
