using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IAddBuilder : IFragmentBuilder
    {
        ITableBuilder Table { get; set; }

        ITableBuilder SetTable(ITableConfig table);

        ICollection<IExpressionBuilder> Expressions { get; }

        IColumnBuilder AddColumn(IColumnConfig column);

        IAddBuilder AddColumns(IEnumerable<IColumnConfig> columns);
    }
}
