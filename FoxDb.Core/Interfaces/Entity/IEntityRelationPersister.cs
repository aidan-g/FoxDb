namespace FoxDb.Interfaces
{
    public interface IEntityRelationPersister<T> where T : IPersistable
    {
        void AddOrUpdate(T item);

        void Delete(T item);
    }
}
