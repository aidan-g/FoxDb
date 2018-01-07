namespace FoxDb.Interfaces
{
    public interface IEntityPopulator<T>
    {
        void Populate(T item, IDatabaseReaderRecord record);
    }
}
