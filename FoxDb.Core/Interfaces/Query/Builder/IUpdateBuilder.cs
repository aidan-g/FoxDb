using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IUpdateBuilder : IFragmentBuilder
    {
        ITableBuilder Table { get; set; }

        ITableBuilder SetTable(ITableConfig table);

        ICollection<IBinaryExpressionBuilder> Expressions { get; }

        IBinaryExpressionBuilder AddColumn(IColumnConfig column);

        IUpdateBuilder AddColumns(IEnumerable<IColumnConfig> columns);
    }
}
