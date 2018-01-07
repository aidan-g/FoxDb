using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IOrderByBuilder : IFragmentTarget
    {
        ICollection<IExpressionBuilder> Expressions { get; }

        IOrderByBuilder AddColumn(IColumnConfig column);

        IOrderByBuilder AddColumns(IEnumerable<IColumnConfig> columns);
    }
}
