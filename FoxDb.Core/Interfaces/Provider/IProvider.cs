using System.Data;

namespace FoxDb.Interfaces
{
    public interface IProvider
    {
        IDbConnection CreateConnection(IDatabase database);

        IDatabaseSchema CreateSchema(IDatabase database);

        IDatabaseQueryFactory CreateQueryFactory(IDatabase database);
    }
}
