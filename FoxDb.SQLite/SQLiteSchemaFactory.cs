using FoxDb.Interfaces;

namespace FoxDb
{
    public class SQLiteSchemaFactory : SqlSchemaFactory
    {
        public SQLiteSchemaFactory(IDatabase database)
            : base(database)
        {
        }

        public override IDatabaseQueryDialect Dialect
        {
            get
            {
                return new SQLiteQueryDialect(this.Database);
            }
        }
    }
}
