using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface ISelectBuilder : IFragmentBuilder
    {
        int Limit { get; set; }

        int Offset { get; set; }

        ICollection<IExpressionBuilder> Expressions { get; }

        IColumnBuilder AddColumn(IColumnConfig column);

        void AddColumns(IEnumerable<IColumnConfig> columns);

        void AddParameters(IEnumerable<IColumnConfig> columns);

        void AddFunction(QueryFunction function, params IExpressionBuilder[] arguments);

        void AddOperator(QueryOperator @operator);
    }
}
