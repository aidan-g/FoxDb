using System.Data;

namespace FoxDb
{
    public abstract class Command : IDbCommand
    {
        public Command(IDbCommand command)
        {
            this.InnerCommand = command;
        }

        public IDbCommand InnerCommand { get; private set; }

        public virtual IDbConnection Connection
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

        public virtual IDbTransaction Transaction
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

        public virtual string CommandText
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

        public virtual int CommandTimeout
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

        public virtual CommandType CommandType
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

        public virtual IDataParameterCollection Parameters
        {
            get
            {
                return this.InnerCommand.Parameters;
            }
        }

        public virtual UpdateRowSource UpdatedRowSource
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

        public virtual IDbDataParameter CreateParameter()
        {
            return this.InnerCommand.CreateParameter();
        }

        public virtual int ExecuteNonQuery()
        {
#if DEBUG
            global::System.Diagnostics.Debug.WriteLine(this.CommandText);
#endif
            return this.InnerCommand.ExecuteNonQuery();
        }

        public virtual object ExecuteScalar()
        {
#if DEBUG
            global::System.Diagnostics.Debug.WriteLine(this.CommandText);
#endif
            return this.InnerCommand.ExecuteScalar();
        }

        public virtual IDataReader ExecuteReader()
        {
#if DEBUG
            global::System.Diagnostics.Debug.WriteLine(this.CommandText);
#endif
            return this.InnerCommand.ExecuteReader();
        }

        public virtual IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return this.InnerCommand.ExecuteReader(behavior);
        }

        public virtual void Prepare()
        {
            this.InnerCommand.Prepare();
        }

        public virtual void Cancel()
        {
            this.InnerCommand.Cancel();
        }

        public virtual void Dispose()
        {
            this.InnerCommand.Dispose();
        }
    }
}
