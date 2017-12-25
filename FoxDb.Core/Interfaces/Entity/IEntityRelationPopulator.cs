namespace FoxDb.Interfaces
{
    public interface IEntityRelationPopulator<T>
    {
        void Populate(T item);
    }
}
