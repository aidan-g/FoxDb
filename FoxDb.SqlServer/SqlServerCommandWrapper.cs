using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace FoxDb
{
    public class SqlServerCommandWrapper : Command
    {
        public SqlServerCommandWrapper(SqlServerProvider provider, SqlServerQueryDialect dialect, DbCommand command)
            : base(command)
        {
            this.Provider = provider;
            this.Dialect = dialect;
        }

        public SqlServerProvider Provider { get; private set; }

        public SqlServerQueryDialect Dialect { get; private set; }

        protected virtual bool HasCommandBatches
        {
            get
            {
                if (string.IsNullOrEmpty(this.CommandText))
                {
                    return false;
                }
                return this.CommandText.IndexOf(this.Dialect.BATCH, StringComparison.OrdinalIgnoreCase) != -1;
            }
        }

        protected virtual IEnumerable<string> CommandBatches
        {
            get
            {
                if (!this.HasCommandBatches)
                {
                    return new[] { this.CommandText };
                }
                else
                {
                    return this.CommandText.Split(this.Dialect.BATCH, StringComparison.OrdinalIgnoreCase, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }

        protected virtual void PerformCommandBatches(Action action)
        {
            var originalCommandText = this.CommandText;
            try
            {
                foreach (var commandText in this.CommandBatches)
                {
                    this.CommandText = commandText;
                    action();
                }
            }
            finally
            {
                this.CommandText = originalCommandText;
            }
        }

        public override int ExecuteNonQuery()
        {
            if (!this.HasCommandBatches)
            {
                return base.ExecuteNonQuery();
            }
            var result = default(int);
            this.PerformCommandBatches(() => result = base.ExecuteNonQuery());
            return result;
        }

        public override object ExecuteScalar()
        {
            if (!this.HasCommandBatches)
            {
                return base.ExecuteScalar();
            }
            var result = default(object);
            this.PerformCommandBatches(() => result = base.ExecuteScalar());
            return result;
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            if (!this.HasCommandBatches)
            {
                return base.ExecuteDbDataReader(behavior);
            }
            var result = default(DbDataReader);
            this.PerformCommandBatches(() => result = base.ExecuteDbDataReader(behavior));
            return result;
        }
    }
}
