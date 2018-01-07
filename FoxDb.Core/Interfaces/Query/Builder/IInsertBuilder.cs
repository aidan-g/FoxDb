using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IInsertBuilder : IFragmentBuilder
    {
        ITableBuilder Table { get; set; }

        IInsertBuilder SetTable(ITableConfig table);

        ICollection<IExpressionBuilder> Expressions { get; }

        IInsertBuilder AddColumn(IColumnConfig column);

        IInsertBuilder AddColumns(IEnumerable<IColumnConfig> columns);
    }
}
