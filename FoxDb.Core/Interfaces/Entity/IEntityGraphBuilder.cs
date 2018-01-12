namespace FoxDb.Interfaces
{
    public interface IEntityGraphBuilder
    {
        IEntityGraph Build(ITableConfig table, IEntityMapper mapper);
    }
}
