namespace FoxDb.Interfaces
{
    public interface IEntityEnumeratorBuffer
    {
        void Update(IDatabaseReaderRecord record);

        bool Exists<T>();

        T Create<T>();

        T Get<T>();

        bool HasKey<T>();

        bool KeyChanged<T>();

        void Remove<T>();
    }
}
