using System.Collections.Generic;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface IOutputBuilder : IFragmentContainer, IFragmentTarget
    {
        IColumnBuilder GetColumn(IColumnConfig column);

        IColumnBuilder AddColumn(IColumnConfig column);

        IOutputBuilder AddColumns(IEnumerable<IColumnConfig> columns);

        IParameterBuilder AddParameter(string name, DbType type, ParameterDirection direction);

        IParameterBuilder AddParameter(IColumnConfig column);

        IOutputBuilder AddParameters(IEnumerable<IColumnConfig> columns);

        IFunctionBuilder AddFunction(QueryFunction function, params IExpressionBuilder[] arguments);

        IWindowFunctionBuilder AddWindowFunction(QueryWindowFunction function, params IExpressionBuilder[] arguments);

        IOperatorBuilder AddOperator(QueryOperator @operator);

        ISubQueryBuilder AddSubQuery(IQueryGraphBuilder query);
    }
}