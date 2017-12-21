namespace FoxDb.Interfaces
{
    public interface IEntityRelationPopulator<T> where T : IPersistable
    {
        void Populate(T item);
    }
}
