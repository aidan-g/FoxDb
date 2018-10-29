using System.Data;

namespace FoxDb.Interfaces
{
    public interface IProvider
    {
        IDbConnection CreateConnection(IDatabase database);

        IDatabaseTranslation CreateTranslation(IDatabase database);

        IDatabaseSchema CreateSchema(IDatabase database);

        IDatabaseQueryFactory CreateQueryFactory(IDatabase database);

        IDatabaseSchemaFactory CreateSchemaFactory(IDatabase database);
    }
}
