using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface ISortBuilder : IFragmentContainer, IFragmentTarget
    {
        IColumnBuilder GetColumn(IColumnConfig column);

        IEnumerable<IColumnBuilder> GetColumns(ITableConfig table);

        IColumnBuilder AddColumn(IColumnConfig column);

        ISortBuilder AddColumns(IEnumerable<IColumnConfig> columns);
    }
}
