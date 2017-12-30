using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IInsertBuilder : IFragmentBuilder
    {
        ITableBuilder Table { get; set; }

        void SetTable(ITableConfig table);

        ICollection<IColumnBuilder> Columns { get; }

        void AddColumns(IEnumerable<IColumnConfig> columns);
    }
}
