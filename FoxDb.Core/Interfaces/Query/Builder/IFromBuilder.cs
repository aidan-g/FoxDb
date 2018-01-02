using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IFromBuilder : IFragmentBuilder
    {
        ICollection<IExpressionBuilder> Expressions { get; }

        void AddTable(ITableConfig table);

        void AddRelation(IRelationConfig relation);

        void AddSubQuery(IQueryGraphBuilder query);
    }
}
