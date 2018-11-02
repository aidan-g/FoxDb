namespace FoxDb.Interfaces
{
    public interface IEntityPersister
    {
        void AddOrUpdate(object item, DatabaseParameterHandler parameters = null);

        void Delete(object item, DatabaseParameterHandler parameters = null);
    }
}
