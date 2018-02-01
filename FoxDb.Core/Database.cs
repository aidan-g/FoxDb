using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace FoxDb
{
    public class Database : Disposable, IDatabase
    {
        private Database()
        {
            this.Config = new Config(this);
        }

        public Database(IProvider provider) : this()
        {
            this.Provider = provider;
            this.Schema = provider.CreateSchema(this);
            this.QueryFactory = provider.CreateQueryFactory(this);
        }

        public IConfig Config { get; private set; }

        public IProvider Provider { get; private set; }

        private IDbConnection _Connection { get; set; }

        public IDbConnection Connection
        {
            get
            {
                if (this._Connection == null)
                {
                    this._Connection = this.Provider.CreateConnection(this);
                }
                switch (this._Connection.State)
                {
                    case ConnectionState.Closed:
                        this._Connection.Open();
                        break;
                }
                return this._Connection;
            }
        }

        public IDatabaseSchema Schema { get; private set; }

        public IDatabaseQueryFactory QueryFactory { get; private set; }

        public IDatabaseQuerySource Source(IDatabaseQueryComposer composer, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return new DatabaseQuerySource(this, composer, parameters, transaction);
        }

        public ITransactionSource BeginTransaction()
        {
            return new TransactionSource(this);
        }

        public ITransactionSource BeginTransaction(IsolationLevel isolationLevel)
        {
            return new TransactionSource(this, isolationLevel);
        }

        public IDatabaseSet<T> Set<T>(IDatabaseQuerySource source)
        {
            return new DatabaseSet<T>(source);
        }

        public int Execute(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            using (var command = this.Connection.CreateCommand(query, parameters, transaction))
            {
                return command.ExecuteNonQuery();
            }
        }

        public T ExecuteScalar<T>(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            using (var command = this.Connection.CreateCommand(query, parameters, transaction))
            {
                return Converter.ChangeType<T>(command.ExecuteScalar());
            }
        }

        [Obsolete]
        public IEnumerable<T> ExecuteEnumerator<T>(ITableConfig table, IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            using (var reader = this.ExecuteReader(query, parameters, transaction))
            {
                var mapper = new EntityMapper(table);
                var enumerable = new EntityCompoundEnumerator(table, mapper, reader);
                foreach (var element in enumerable.AsEnumerable<T>())
                {
                    yield return element;
                }
            }
        }

        public IDatabaseReader ExecuteReader(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            var command = this.Connection.CreateCommand(query, parameters, transaction);
            return new DatabaseReader(command);
        }

        protected override void Dispose(bool disposing)
        {
            if (this._Connection != null)
            {
                this._Connection.Dispose();
                this._Connection = null;
            }
            base.Dispose(disposing);
        }
    }
}
