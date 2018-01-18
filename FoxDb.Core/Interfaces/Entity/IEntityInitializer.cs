namespace FoxDb.Interfaces
{
    public interface IEntityInitializer
    {
        ITableConfig Table { get; }

        IEntityMapper Mapper { get; }

        void Initialize(object item);
    }
}
