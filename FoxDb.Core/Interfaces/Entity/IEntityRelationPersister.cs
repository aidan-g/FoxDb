namespace FoxDb.Interfaces
{
    public interface IEntityRelationPersister<T>
    {
        void AddOrUpdate(T item);

        void Delete(T item);
    }
}
