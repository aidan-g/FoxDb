using System.Data;

namespace FoxDb.Interfaces
{
    public interface IProvider
    {
        void CreateDatabase(string name);

        void DeleteDatabase(string name);

        IDbConnection CreateConnection(IDatabase database);

        IDatabaseTranslation CreateTranslation(IDatabase database);

        IDatabaseSchema CreateSchema(IDatabase database);

        IDatabaseQueryFactory CreateQueryFactory(IDatabase database);

        IDatabaseSchemaFactory CreateSchemaFactory(IDatabase database);
    }
}
