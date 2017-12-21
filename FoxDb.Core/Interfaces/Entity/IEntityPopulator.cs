namespace FoxDb.Interfaces
{
    public interface IEntityPopulator<T> where T : IPersistable
    {
        void Populate(T item, IDatabaseReaderRecord record);
    }
}
