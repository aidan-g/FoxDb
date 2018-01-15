using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface ISelectBuilder : IFragmentBuilder
    {
        ICollection<IExpressionBuilder> Expressions { get; }

        IColumnBuilder AddColumn(IColumnConfig column);

        ISelectBuilder AddColumns(IEnumerable<IColumnConfig> columns);

        ISelectBuilder AddParameters(IEnumerable<IColumnConfig> columns);

        IFunctionBuilder AddFunction(QueryFunction function, params IExpressionBuilder[] arguments);

        IOperatorBuilder AddOperator(QueryOperator @operator);
    }
}
