using System.Collections.Generic;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQuerySource
    {
        IDatabase Database { get; }

        ITableConfig Table { get; }

        bool CanRead { get; }

        bool CanWrite { get; }

        IEntityMapper Mapper { get; }

        IEntityRelationQueryComposer Composer { get; }

        IQueryGraphBuilder Fetch { get; set; }

        IEnumerable<IQueryGraphBuilder> Add { get; set; }

        IQueryGraphBuilder Update { get; set; }

        IQueryGraphBuilder Delete { get; set; }

        DatabaseParameterHandler Parameters { get; set; }

        IDbTransaction Transaction { get; }
    }
}
