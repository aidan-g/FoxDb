using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IEntityMapper
    {
        ITableConfig Table { get; }

        IEnumerable<ITableConfig> Tables { get; }

        IEnumerable<IRelationConfig> Relations { get; }
    }
}
