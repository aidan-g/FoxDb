using System.Data;

namespace FoxDb.Interfaces
{
    public interface IProvider
    {
        IDbConnection CreateConnection(IDatabase database);

        IDatabaseQueryFactory CreateQueryFactory(IDatabase database);
    }
}
