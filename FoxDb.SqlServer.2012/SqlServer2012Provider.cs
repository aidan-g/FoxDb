using System.Data.SqlClient;
using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlServer2012Provider : SqlServerProvider
    {
        public SqlServer2012Provider(string dataSource, string initialCatalog) : base(dataSource, initialCatalog)
        {

        }

        public SqlServer2012Provider(string dataSource, string initialCatalog, string user, string password) : base(dataSource, initialCatalog, user, password)
        {

        }

        public SqlServer2012Provider(SqlConnectionStringBuilder builder) : base(builder)
        {

        }

        public override IDatabaseQueryFactory CreateQueryFactory(IDatabase database)
        {
            return new SqlServer2012QueryFactory(database);
        }
    }
}
