using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQuerySource<T> where T : IPersistable
    {
        IDatabase Database { get; }

        IDatabaseQuery Select { get; set; }

        IDatabaseQuery Insert { get; set; }

        IDatabaseQuery Update { get; set; }

        IDatabaseQuery Delete { get; set; }

        DatabaseParameterHandler Parameters { get; set; }

        IDbTransaction Transaction { get; set; }
    }
}
