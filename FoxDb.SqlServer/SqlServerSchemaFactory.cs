using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlServerSchemaFactory : SqlSchemaFactory
    {
        public SqlServerSchemaFactory(IDatabase database)
            : base(database)
        {
        }

        public override IDatabaseQueryDialect Dialect
        {
            get
            {
                return new SqlServerQueryDialect(this.Database);
            }
        }
    }
}
