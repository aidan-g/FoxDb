using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IAddBuilder : IFragmentContainer, IFragmentBuilder
    {
        ITableBuilder Table { get; set; }

        ITableBuilder SetTable(ITableConfig table);

        IColumnBuilder GetColumn(IColumnConfig column);

        IColumnBuilder AddColumn(IColumnConfig column);

        IAddBuilder AddColumns(IEnumerable<IColumnConfig> columns);
    }
}
