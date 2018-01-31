namespace FoxDb.Interfaces
{
    public interface IEntityPersister
    {
        void AddOrUpdate(object item);

        void AddOrUpdate(object item, PersistenceFlags flags);

        void Delete(object item);

        void Delete(object item, PersistenceFlags flags);
    }
}
