using System.Data;
using System.Data.Common;

namespace FoxDb
{
    public class SqlServerConnectionWrapper : Connection
    {
        public SqlServerConnectionWrapper(SqlServerProvider provider, SqlServerQueryDialect dialect, IDbConnection connection)
            : base(connection)
        {
            this.Provider = provider;
            this.Dialect = dialect;
        }

        public SqlServerProvider Provider { get; private set; }

        public SqlServerQueryDialect Dialect { get; private set; }

        public override IDbCommand CreateCommand()
        {
            return new SqlServerCommandWrapper(this.Provider, this.Dialect, (DbCommand)base.CreateCommand());
        }
    }
}
