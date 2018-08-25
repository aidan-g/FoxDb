using System.Data;

namespace FoxDb.Interfaces
{
    public interface IProvider
    {
        IDbConnection CreateConnection(IDatabase database);

        IDatabaseSchema CreateSchema(IDatabase database);

        IDatabaseQueryFactory CreateQueryFactory(IDatabase database);

        IDatabaseSchemaFactory CreateSchemaFactory(IDatabase database);

        DbType GetDbType(IDataParameter parameter, object value);

        object GetDbValue(IDataParameter parameter, object value);
    }
}
