namespace FoxDb.Interfaces.Entity
{
    public interface IEntityFactory<T>
    {
        T Create();
    }
}
