using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IOrderByBuilder : IFragmentTarget
    {
        ICollection<IExpressionBuilder> Expressions { get; }

        IColumnBuilder AddColumn(IColumnConfig column);

        IOrderByBuilder AddColumns(IEnumerable<IColumnConfig> columns);
    }
}
