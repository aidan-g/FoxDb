using FoxDb.Interfaces;
using System;
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
            this.CommandCache = new DatabaseCommandCache(database);
            this.IsolationLevel = isolationLevel;
        }

        public IDatabase Database { get; private set; }

        public IDatabaseCommandCache CommandCache { get; private set; }

        public IsolationLevel? IsolationLevel { get; private set; }

        public IDbTransaction Transaction { get; private set; }

        public bool HasTransaction
        {
            get
            {
                return this.Transaction != null;
            }
        }

        public void Bind(IDbCommand command)
        {
            if (!this.HasTransaction)
            {
                this.Begin();
            }
            command.Transaction = this.Transaction;
        }

        public void Begin()
        {
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
            if (!this.HasTransaction)
            {
                throw new InvalidOperationException("No such transaction.");
            }
            this.Transaction.Commit();
            this.Reset();
        }

        public void Rollback()
        {
            if (!this.HasTransaction)
            {
                throw new InvalidOperationException("No such transaction.");
            }
            this.Transaction.Rollback();
            this.Reset();
        }

        public void Reset()
        {
            this.CommandCache.Clear();
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
