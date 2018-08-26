using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface ICreateBuilder : IFragmentContainer, IFragmentBuilder
    {
        ITableBuilder Table { get; set; }

        ITableBuilder SetTable(ITableConfig table);

        IColumnBuilder GetColumn(IColumnConfig column);

        IColumnBuilder AddColumn(IColumnConfig column);

        ICreateBuilder AddColumns(IEnumerable<IColumnConfig> columns);

        IIndexBuilder GetIndex(IIndexConfig index);

        IIndexBuilder AddIndex(IIndexConfig index);

        ICreateBuilder AddIndexes(IEnumerable<IIndexConfig> indexes);
    }
}
