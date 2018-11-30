using System.Data;
using System.Data.Common;

namespace FoxDb
{
    public abstract class Command : DbCommand
    {
        public Command(DbCommand command)
        {
            this.InnerCommand = command;
        }

        public DbCommand InnerCommand { get; private set; }

        public override string CommandText
        {
            get
            {
                return this.InnerCommand.CommandText;
            }
            set
            {
                this.InnerCommand.CommandText = value;
            }
        }

        public override int CommandTimeout
        {
            get
            {
                return this.InnerCommand.CommandTimeout;
            }
            set
            {
                this.InnerCommand.CommandTimeout = value;
            }
        }

        public override CommandType CommandType
        {
            get
            {
                return this.InnerCommand.CommandType;
            }
            set
            {
                this.InnerCommand.CommandType = value;
            }
        }

        protected override DbConnection DbConnection
        {
            get
            {
                return this.InnerCommand.Connection;
            }
            set
            {
                this.InnerCommand.Connection = value;
            }
        }

        protected override DbParameterCollection DbParameterCollection
        {
            get
            {
                return this.InnerCommand.Parameters;
            }
        }

        protected override DbTransaction DbTransaction
        {
            get
            {
                return this.InnerCommand.Transaction;
            }
            set
            {
                this.InnerCommand.Transaction = value;
            }
        }

        public override bool DesignTimeVisible
        {
            get
            {
                return this.InnerCommand.DesignTimeVisible;
            }
            set
            {
                this.InnerCommand.DesignTimeVisible = value;
            }
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get
            {
                return this.InnerCommand.UpdatedRowSource;
            }
            set
            {
                this.InnerCommand.UpdatedRowSource = value;
            }
        }

        protected override DbParameter CreateDbParameter()
        {
            return this.InnerCommand.CreateParameter();
        }

        public override int ExecuteNonQuery()
        {
            return this.InnerCommand.ExecuteNonQuery();
        }

        public override object ExecuteScalar()
        {
            return this.InnerCommand.ExecuteScalar();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return this.InnerCommand.ExecuteReader(behavior);
        }

        public override void Prepare()
        {
            this.InnerCommand.Prepare();
        }

        public override void Cancel()
        {
            this.InnerCommand.Cancel();
        }
    }
}
