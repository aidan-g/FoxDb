using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface ISortBuilder : IFragmentContainer, IFragmentTarget
    {
        IColumnBuilder AddColumn(IColumnConfig column);

        ISortBuilder AddColumns(IEnumerable<IColumnConfig> columns);
    }
}
