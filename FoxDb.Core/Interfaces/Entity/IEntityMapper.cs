using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IEntityMapper
    {
        bool IncludeRelations { get; }

        IEnumerable<ITableConfig> Tables { get; }

        IEnumerable<IRelationConfig> Relations { get; }

        IEnumerable<IEntityColumnMap> GetColumns(ITableConfig table);

        IEntityColumnMap GetColumn(IColumnConfig column);

        IDatabaseQuery Select { get; }
    }
}
