namespace FoxDb.Interfaces
{
    public interface IEntityGraphBuilder
    {
        IEntityGraph Build<T>(ITableConfig table, IEntityMapper mapper);
    }
}
