using FoxDb.Interfaces;
using System.Data;

namespace FoxDb
{
    public class TransactionSource : Disposable, ITransactionSource
    {
        public TransactionSource(IDatabase database) : this(database, null)
        {

        }

        public TransactionSource(IDatabase database, IsolationLevel? isolationLevel)
        {
            this.Database = database;
            this.IsolationLevel = isolationLevel;
        }

        public IDatabase Database { get; private set; }

        public IsolationLevel? IsolationLevel { get; private set; }

        public IDbTransaction Transaction { get; private set; }

        public void Bind(IDbCommand command)
        {
            if (this.Transaction == null)
            {
                this.Begin();
            }
            command.Transaction = this.Transaction;
        }

        public void Begin()
        {
            if (this.Transaction != null)
            {
                return;
            }
            if (this.IsolationLevel.HasValue)
            {
                this.Transaction = this.Database.Connection.BeginTransaction(this.IsolationLevel.Value);
            }
            else
            {
                this.Transaction = this.Database.Connection.BeginTransaction();
            }
        }

        public void Commit()
        {
            this.Transaction.Commit();
            this.Reset();
        }

        public void Rollback()
        {
            this.Transaction.Rollback();
            this.Reset();
        }

        public void Reset()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Dispose();
                this.Transaction = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            this.Reset();
            base.Dispose(disposing);
        }
    }
}
