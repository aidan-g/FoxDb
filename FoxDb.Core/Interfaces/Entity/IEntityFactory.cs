namespace FoxDb.Interfaces
{
    public interface IEntityFactory<T> where T : IPersistable
    {
        T Create();
    }
}
