namespace FoxDb.Interfaces
{
    public interface IEntityEnumeratorBuffer
    {
        void Update(IDatabaseReaderRecord record);

        bool Exists(ITableConfig table);

        object Create(ITableConfig table);

        object Get(ITableConfig table);

        bool HasKey(ITableConfig table);

        bool HasKey(ITableConfig table, out object key);

        bool KeyChanged(ITableConfig table);

        void Remove(ITableConfig table);
    }
}
