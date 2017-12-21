namespace FoxDb.Interfaces
{
    public interface IEntityPersister<T> where T : IPersistable
    {
        void AddOrUpdate(T item);

        void Delete(T item);
    }
}
