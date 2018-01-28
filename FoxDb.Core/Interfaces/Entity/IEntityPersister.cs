namespace FoxDb.Interfaces
{
    public interface IEntityPersister<T>
    {
        void AddOrUpdate(T item);

        void AddOrUpdate(T item, PersistenceFlags flags);

        void Delete(T item);

        void Delete(T item, PersistenceFlags flags);
    }
}
