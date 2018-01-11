using System.Collections.Generic;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQuerySource
    {
        IDatabase Database { get; }

        ITableConfig Table { get; }

        bool CanRead { get; }

        bool CanSearch { get; }

        bool CanWrite { get; }

        IEntityMapper Mapper { get; }

        IQueryGraphBuilder Select { get; set; }

        IQueryGraphBuilder Find { get; set; }

        IEnumerable<IQueryGraphBuilder> Insert { get; set; }

        IQueryGraphBuilder Update { get; set; }

        IQueryGraphBuilder Delete { get; set; }

        DatabaseParameterHandler Parameters { get; set; }

        IDbTransaction Transaction { get; }
    }
}
