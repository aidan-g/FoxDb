using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IFilterBuilder : IFragmentContainer, IFragmentTarget
    {
        int? Limit { get; set; }

        LimitType LimitType { get; set; }

        int? Offset { get; set; }

        OffsetType OffsetType { get; set; }

        IBinaryExpressionBuilder Add();

        IBinaryExpressionBuilder GetColumn(IColumnConfig column);

        IBinaryExpressionBuilder AddColumn(IColumnConfig column);

        IBinaryExpressionBuilder GetColumn(IColumnConfig leftColumn, IColumnConfig rightColumn);

        IBinaryExpressionBuilder AddColumn(IColumnConfig leftColumn, IColumnConfig rightColumn);

        void AddColumns(IEnumerable<IColumnConfig> columns);

        IFunctionBuilder AddFunction(IFunctionBuilder function);

        IFunctionBuilder AddFunction(QueryFunction function, params IExpressionBuilder[] arguments);
    }

    public enum LimitType : byte
    {
        None = 0,
        Percent = 1
    }

    public enum OffsetType : byte
    {
        None = 0
    }
}
