using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

namespace FoxDb
{
    public class TransactionSource : Disposable, ITransactionSource
    {
#if DEBUG
        static TransactionSource()
        {
            Transactions = new ConditionalWeakTable<IDatabase, Stack<ITransactionSource>>();
        }

        private static ConditionalWeakTable<IDatabase, Stack<ITransactionSource>> Transactions { get; set; }
#endif

        public TransactionSource(IDatabase database)
        {
            this.Database = database;
            this.CommandCache = new DatabaseCommandCache(database);
#if DEBUG
            Transactions.GetOrCreateValue(database).Push(this);
#endif
        }

        public TransactionSource(IDatabase database, IsolationLevel? isolationLevel)
            : this(database)
        {
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

        public virtual void Begin()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            if (this.HasTransaction)
            {
                throw new InvalidOperationException("Transaction has clready been created.");
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

        public virtual void Commit()
        {
            if (!this.HasTransaction)
            {
                throw new InvalidOperationException("No such transaction.");
            }
            this.Transaction.Commit();
            this.Reset();
        }

        public virtual void Rollback()
        {
            if (!this.HasTransaction)
            {
                throw new InvalidOperationException("No such transaction.");
            }
            this.Transaction.Rollback();
            this.Reset();
        }

        public virtual void Reset()
        {
            this.CommandCache.Clear();
            if (this.Transaction != null)
            {
                this.Transaction.Dispose();
                this.Transaction = null;
            }
        }

        protected override void OnDisposing()
        {
#if DEBUG
            var transactions = default(Stack<ITransactionSource>);
            if (Transactions.TryGetValue(this.Database, out transactions))
            {
                if (!object.ReferenceEquals(this, transactions.Pop()))
                {
                    Transactions.Remove(this.Database);
                    throw new InvalidOperationException(string.Format("{0} was disposed out of sequence.", this.GetType().Name));
                }
            }
#endif
            this.Reset();
            base.OnDisposing();
        }
    }
}
