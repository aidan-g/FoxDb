namespace FoxDb.Interfaces
{
    public interface IEntityInitializer<T>
    {
        void Initialize(T item);
    }
}
