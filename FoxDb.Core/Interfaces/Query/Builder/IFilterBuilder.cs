using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IFilterBuilder : IFragmentTarget
    {
        int Limit { get; set; }

        int Offset { get; set; }

        ICollection<IExpressionBuilder> Expressions { get; }

        IBinaryExpressionBuilder Add();

        IBinaryExpressionBuilder AddColumn(IColumnConfig column);

        IBinaryExpressionBuilder AddColumn(IColumnConfig leftColumn, IColumnConfig rightColumn);

        void AddColumns(IEnumerable<IColumnConfig> columns);

        IFunctionBuilder AddFunction(IFunctionBuilder function);
    }
}
