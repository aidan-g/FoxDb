using System;
using System.Collections.Generic;
using System.Data;

namespace FoxDb
{
    public class SqlServerCommandWrapper : Command
    {
        public SqlServerCommandWrapper(SqlServerProvider provider, SqlServerQueryDialect dialect, IDbCommand command)
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
                    var index1 = 0;
                    var index2 = 0;
                    var commandBatches = new List<string>();
                    while ((index2 = this.CommandText.IndexOf(this.Dialect.BATCH, index1, StringComparison.OrdinalIgnoreCase)) != -1)
                    {
                        commandBatches.Add(this.CommandText.Substring(index1, index2 - index1).Trim());
                        index1 = index2 + this.Dialect.BATCH.Length;
                    }
                    if (index1 < this.CommandText.Length)
                    {
                        commandBatches.Add(this.CommandText.Substring(index1).Trim());
                    }
                    return commandBatches;
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

        public override IDataReader ExecuteReader()
        {
            if (!this.HasCommandBatches)
            {
                return base.ExecuteReader();
            }
            var result = default(IDataReader);
            this.PerformCommandBatches(() => result = base.ExecuteReader());
            return result;
        }

        public override IDataReader ExecuteReader(CommandBehavior behavior)
        {
            if (!this.HasCommandBatches)
            {
                return base.ExecuteReader(behavior);
            }
            var result = default(IDataReader);
            this.PerformCommandBatches(() => result = base.ExecuteReader(behavior));
            return result;
        }
    }
}
