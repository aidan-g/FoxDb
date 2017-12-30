using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IWhereBuilder : IFragmentBuilder, IFragmentTarget
    {
        ICollection<IBinaryExpressionBuilder> Expressions { get; }

        IBinaryExpressionBuilder AddColumn(IColumnConfig column);

        void AddColumns(IEnumerable<IColumnConfig> columns);
    }
}
