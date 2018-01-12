namespace FoxDb.Interfaces
{
    public interface IEntityEnumeratorBuffer
    {
        void Update(IDatabaseReaderRecord record);

        bool Exists<T>();

        T Create<T>(ITableConfig table);

        T Get<T>();

        bool HasKey(ITableConfig table);

        bool HasKey(ITableConfig table, out object key);

        bool KeyChanged<T>(ITableConfig table);

        void Remove<T>();
    }
}
