namespace FoxDb.Interfaces
{
    public interface IEntityInitializer
    {
        ITableConfig Table { get; }

        void Initialize(object item);
    }
}
