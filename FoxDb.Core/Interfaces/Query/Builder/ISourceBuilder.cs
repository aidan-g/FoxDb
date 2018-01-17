namespace FoxDb.Interfaces
{
    public interface ISourceBuilder : IFragmentContainer, IFragmentBuilder
    {
        ITableBuilder GetTable(ITableConfig table);

        ITableBuilder AddTable(ITableConfig table);

        IRelationBuilder GetRelation(IRelationConfig relation);

        IRelationBuilder AddRelation(IRelationConfig relation);

        ISubQueryBuilder GetSubQuery(IQueryGraphBuilder query);

        ISubQueryBuilder AddSubQuery(IQueryGraphBuilder query);
    }
}

