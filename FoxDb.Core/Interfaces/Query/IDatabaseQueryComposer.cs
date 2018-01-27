namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryComposer
    {
        ITableConfig Table { get; }

        IQueryGraphBuilder Fetch { get; }

        IQueryGraphBuilder Add { get; }

        IQueryGraphBuilder Update { get; }

        IQueryGraphBuilder Delete { get; }
    }
}
