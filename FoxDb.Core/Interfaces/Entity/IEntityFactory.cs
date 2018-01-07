namespace FoxDb.Interfaces
{
    public interface IEntityFactory
    {
        object Create(IDatabaseReaderRecord record);
    }

    public interface IEntityFactory<T> : IEntityFactory
    {
        new T Create(IDatabaseReaderRecord record);
    }
}
