namespace FoxDb.Interfaces
{
    public interface IEntityPersister<T>
    {
        void AddOrUpdate(T item);

        void Delete(T item);
    }
}
