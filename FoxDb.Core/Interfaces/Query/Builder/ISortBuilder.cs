using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface ISortBuilder : IFragmentTarget
    {
        ICollection<IExpressionBuilder> Expressions { get; }

        IColumnBuilder AddColumn(IColumnConfig column);

        ISortBuilder AddColumns(IEnumerable<IColumnConfig> columns);
    }
}
