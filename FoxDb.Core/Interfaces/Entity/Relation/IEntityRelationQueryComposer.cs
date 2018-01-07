namespace FoxDb.Interfaces
{
    public interface IEntityRelationQueryComposer
    {
        IQueryGraphBuilder Select { get; }

        IQueryGraphBuilder Find { get; }
    }
}
