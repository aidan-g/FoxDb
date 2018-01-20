using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface ISourceBuilder : IFragmentContainer, IFragmentBuilder
    {
        IEnumerable<ITableBuilder> Tables { get; }

        IEnumerable<IRelationBuilder> Relations { get; }

        IEnumerable<ISubQueryBuilder> SubQueries { get; }

        ITableBuilder GetTable(ITableConfig table);

        ITableBuilder AddTable(ITableConfig table);

        IRelationBuilder GetRelation(IRelationConfig relation);

        IRelationBuilder AddRelation(IRelationConfig relation);

        ISubQueryBuilder GetSubQuery(IQueryGraphBuilder query);

        ISubQueryBuilder AddSubQuery(IQueryGraphBuilder query);
    }
}

