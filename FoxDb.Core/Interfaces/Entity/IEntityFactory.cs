namespace FoxDb.Interfaces
{
    public interface IEntityFactory<T>
    {
        T Create();
    }
}
