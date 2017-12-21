namespace FoxDb.Interfaces
{
    public interface IEntityPropertyWriter<T>
    {
        void Write(T item, string name, object value);
    }
}
