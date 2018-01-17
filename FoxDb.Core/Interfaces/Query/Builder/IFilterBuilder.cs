﻿using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IFilterBuilder : IFragmentContainer, IFragmentTarget
    {
        int Limit { get; set; }

        int Offset { get; set; }

        IBinaryExpressionBuilder Add();

        IFilterBuilder Add(IFilterBuilder builder);

        IBinaryExpressionBuilder GetColumn(IColumnConfig column);

        IBinaryExpressionBuilder AddColumn(IColumnConfig column);

        IBinaryExpressionBuilder GetColumn(IColumnConfig leftColumn, IColumnConfig rightColumn);

        IBinaryExpressionBuilder AddColumn(IColumnConfig leftColumn, IColumnConfig rightColumn);

        void AddColumns(IEnumerable<IColumnConfig> columns);

        IFunctionBuilder AddFunction(IFunctionBuilder function);

        IFunctionBuilder AddFunction(QueryFunction function, params IExpressionBuilder[] arguments);
    }
}