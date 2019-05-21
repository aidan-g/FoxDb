using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IWithBuilder : IFragmentContainer, IFragmentTarget
    {
        IEnumerable<ICommonTableExpressionBuilder> CommonTableExpressions { get; }

        ICommonTableExpressionBuilder GetCommonTableExpression(IQueryGraphBuilder query);

        ICommonTableExpressionBuilder GetCommonTableExpression(IEnumerable<IQueryGraphBuilder> queries);

        ICommonTableExpressionBuilder AddCommonTableExpression(IQueryGraphBuilder query);

        ICommonTableExpressionBuilder AddCommonTableExpression(IEnumerable<IQueryGraphBuilder> queries);
    }
}
