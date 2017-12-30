using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IUpdateBuilder : IFragmentBuilder
    {
        ITableBuilder Table { get; set; }

        void SetTable(ITableConfig table);

        ICollection<IBinaryExpressionBuilder> Expressions { get; }

        IBinaryExpressionBuilder AddColumn(IColumnConfig column);

        void AddColumns(IEnumerable<IColumnConfig> columns);
    }
}
