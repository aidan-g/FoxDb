using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlCeSchemaFactory : SqlSchemaFactory
    {
        public SqlCeSchemaFactory(IDatabase database)
            : base(database)
        {
        }

        public override IDatabaseQueryDialect Dialect
        {
            get
            {
                return new SqlCeQueryDialect(this.Database);
            }
        }
    }
}
