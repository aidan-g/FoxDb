using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IAggregateBuilder : IFragmentContainer, IFragmentTarget
    {
        IColumnBuilder AddColumn(IColumnConfig column);

        IAggregateBuilder AddColumns(IEnumerable<IColumnConfig> columns);
    }
}