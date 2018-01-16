using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IOutputBuilder : IFragmentBuilder
    {
        ICollection<IExpressionBuilder> Expressions { get; }

        IColumnBuilder AddColumn(IColumnConfig column);

        IOutputBuilder AddColumns(IEnumerable<IColumnConfig> columns);

        IOutputBuilder AddParameters(IEnumerable<IColumnConfig> columns);

        IFunctionBuilder AddFunction(QueryFunction function, params IExpressionBuilder[] arguments);

        IOperatorBuilder AddOperator(QueryOperator @operator);
    }
}