using System.Data;

namespace FoxDb
{
    public abstract class Connection : IDbConnection
    {
        public Connection(IDbConnection connection)
        {
            this.InnerConnection = connection;
        }

        public IDbConnection InnerConnection { get; private set; }

        public virtual string ConnectionString
        {
            get
            {
                return this.InnerConnection.ConnectionString;
            }
            set
            {
                this.InnerConnection.ConnectionString = value;
            }
        }

        public virtual int ConnectionTimeout
        {
            get
            {
                return this.InnerConnection.ConnectionTimeout;
            }
        }

        public virtual string Database
        {
            get
            {
                return this.InnerConnection.Database;
            }
        }

        public virtual ConnectionState State
        {
            get
            {
                return this.InnerConnection.State;
            }
        }

        public virtual IDbTransaction BeginTransaction()
        {
            return this.InnerConnection.BeginTransaction();
        }

        public virtual IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return this.InnerConnection.BeginTransaction(isolationLevel);
        }

        public virtual void ChangeDatabase(string databaseName)
        {
            this.InnerConnection.ChangeDatabase(databaseName);
        }

        public virtual IDbCommand CreateCommand()
        {
            return this.InnerConnection.CreateCommand();
        }

        public virtual void Open()
        {
            this.InnerConnection.Open();
        }

        public virtual void Close()
        {
            this.InnerConnection.Close();
        }

        public virtual void Dispose()
        {
            this.InnerConnection.Dispose();
        }
    }
}
