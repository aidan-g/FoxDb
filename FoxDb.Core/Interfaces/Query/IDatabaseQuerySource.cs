namespace FoxDb.Interfaces
{
    public interface IDatabaseQuerySource
    {
        IDatabase Database { get; }

        ITableConfig Table { get; }

        DatabaseParameterHandler Parameters { get; set; }

        ITransactionSource Transaction { get; }

        IEntityMapper Mapper { get; }

        IEntityRelationQueryComposer Composer { get; }

        IQueryGraphBuilder Fetch { get; set; }

        IQueryGraphBuilder Add { get; set; }

        IQueryGraphBuilder Update { get; set; }

        IQueryGraphBuilder Delete { get; set; }

        void Reset();
    }
}
