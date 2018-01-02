using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IWhereBuilder : IFragmentTarget
    {
        ICollection<IExpressionBuilder> Expressions { get; }

        IBinaryExpressionBuilder AddColumn(IColumnConfig column);

        void AddColumns(IEnumerable<IColumnConfig> columns);
    }
}
