namespace FoxDb.Interfaces
{
    public interface IEntityPropertyReader<T>
    {
        object Read(T item, string name);
    }
}
