using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IFromBuilder : IFragmentBuilder
    {
        ICollection<IExpressionBuilder> Expressions { get; }

        ITableBuilder AddTable(ITableConfig table);

        IRelationBuilder AddRelation(IRelationConfig relation);

        ISubQueryBuilder AddSubQuery(IQueryGraphBuilder query);
    }
}
