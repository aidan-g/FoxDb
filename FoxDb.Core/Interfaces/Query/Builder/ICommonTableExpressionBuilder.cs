using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface ICommonTableExpressionBuilder : IExpressionBuilder, IFragmentContainer, IFragmentTarget
    {
        ICollection<string> ColumnNames { get; }

        IEnumerable<ISubQueryBuilder> SubQueries { get; }

        ISubQueryBuilder GetSubQuery(IQueryGraphBuilder query);

        ISubQueryBuilder AddSubQuery(IQueryGraphBuilder query);
    }
}
