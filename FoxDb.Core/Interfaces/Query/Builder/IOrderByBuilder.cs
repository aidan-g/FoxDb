using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IOrderByBuilder : IFragmentBuilder
    {
        ICollection<IColumnBuilder> Columns { get; }

        IColumnBuilder AddColumn(IColumnConfig column);

        void AddColumns(IEnumerable<IColumnConfig> columns);
    }
}
