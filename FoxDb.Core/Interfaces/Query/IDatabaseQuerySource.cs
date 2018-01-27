namespace FoxDb.Interfaces
{
    public interface IDatabaseQuerySource : ICloneable<IDatabaseQuerySource>
    {
        IDatabase Database { get; }

        IDatabaseQueryComposer Composer { get; }

        DatabaseParameterHandler Parameters { get; set; }

        ITransactionSource Transaction { get; }

        IQueryGraphBuilder Fetch { get; set; }

        IQueryGraphBuilder Add { get; set; }

        IQueryGraphBuilder Update { get; set; }

        IQueryGraphBuilder Delete { get; set; }

        void Reset();
    }
}
