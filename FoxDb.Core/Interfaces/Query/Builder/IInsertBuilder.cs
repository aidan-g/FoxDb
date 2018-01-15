using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IInsertBuilder : IFragmentBuilder
    {
        ITableBuilder Table { get; set; }

        ITableBuilder SetTable(ITableConfig table);

        ICollection<IExpressionBuilder> Expressions { get; }

        IColumnBuilder AddColumn(IColumnConfig column);

        IInsertBuilder AddColumns(IEnumerable<IColumnConfig> columns);
    }
}
