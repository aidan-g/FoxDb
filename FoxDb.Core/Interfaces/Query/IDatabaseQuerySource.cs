using System.Collections.Generic;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQuerySource
    {
        IDatabase Database { get; }

        IEntityMapper Mapper { get; }

        IQueryGraphBuilder Select { get; set; }

        IEnumerable<IQueryGraphBuilder> Insert { get; set; }

        IQueryGraphBuilder Update { get; set; }

        IQueryGraphBuilder Delete { get; set; }

        DatabaseParameterHandler Parameters { get; set; }

        IDbTransaction Transaction { get; set; }
    }
}
