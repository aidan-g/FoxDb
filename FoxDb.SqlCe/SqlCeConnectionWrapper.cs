using System.Data;
using System.Data.Common;

namespace FoxDb
{
    public class SqlCeConnectionWrapper : Connection
    {
        public SqlCeConnectionWrapper(SqlCeProvider provider, SqlCeQueryDialect dialect, IDbConnection connection)
            : base(connection)
        {
            this.Provider = provider;
            this.Dialect = dialect;
        }

        public SqlCeProvider Provider { get; private set; }

        public SqlCeQueryDialect Dialect { get; private set; }

        public override IDbCommand CreateCommand()
        {
            return new SqlCeCommandWrapper(this.Provider, this.Dialect, (DbCommand)base.CreateCommand());
        }
    }
}
