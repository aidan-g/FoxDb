using FoxDb.Interfaces;
using System.Data;

namespace FoxDb
{
    public abstract class Provider : IProvider
    {
        public abstract bool CheckDatabase();

        public abstract void CreateDatabase(string name);

        public abstract void DeleteDatabase(string name);

        public abstract IDbConnection CreateConnection(IDatabase database);

        public virtual IDatabaseTranslation CreateTranslation(IDatabase database)
        {
            return new DatabaseTranslation(database);
        }

        public abstract IDatabaseQueryFactory CreateQueryFactory(IDatabase database);

        public abstract IDatabaseSchema CreateSchema(IDatabase database);

        public abstract IDatabaseSchemaFactory CreateSchemaFactory(IDatabase database);
    }
}
