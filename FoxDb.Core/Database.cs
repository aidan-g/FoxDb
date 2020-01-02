using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FoxDb
{
    public partial class Database : Disposable, IDatabase
    {
        private Database()
        {
            this.Members = new DynamicMethod<Database>();
            this.Config = new Config(this);
            this.QueryCache = new DatabaseQueryCache(this);
        }

        public Database(IProvider provider)
            : this()
        {
            this.Provider = provider;
            this.Translation = provider.CreateTranslation(this);
            this.Schema = provider.CreateSchema(this);
            this.QueryFactory = provider.CreateQueryFactory(this);
            this.SchemaFactory = provider.CreateSchemaFactory(this);
        }

        protected DynamicMethod<Database> Members { get; private set; }

        public IConfig Config { get; private set; }

        public IProvider Provider { get; private set; }

        private IDbConnection _Connection { get; set; }

        public IDbConnection Connection
        {
            get
            {
                if (this._Connection == null)
                {
                    if (this.IsDisposed)
                    {
                        throw new ObjectDisposedException(this.GetType().FullName);
                    }
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

        public IDatabaseTranslation Translation { get; private set; }

        public IDatabaseSchema Schema { get; private set; }

        public IDatabaseQueryCache QueryCache { get; private set; }

        public IDatabaseQueryFactory QueryFactory { get; private set; }

        public IDatabaseSchemaFactory SchemaFactory { get; private set; }

        public IDatabaseQuerySource Source(IDatabaseQueryComposer composer, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return new DatabaseQuerySource(this, composer, parameters, transaction);
        }

        public virtual ITransactionSource BeginTransaction()
        {
            return new TransactionSource(this);
        }

        public virtual ITransactionSource BeginTransaction(IsolationLevel isolationLevel)
        {
            return new TransactionSource(this, isolationLevel);
        }

        public IDatabaseSet Set(Type type, IDatabaseQuerySource source)
        {
            return (IDatabaseSet)this.Members.Invoke(this, "Set", type, source);
        }

        public IDatabaseSet<T> Set<T>(IDatabaseQuerySource source)
        {
            return new DatabaseSet<T>(source);
        }

        public int Execute(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            if (string.IsNullOrEmpty(query.CommandText))
            {
                throw new InvalidOperationException("The query is empty.");
            }
            return this.CreateCommand(
                query,
                DatabaseCommandFlags.None,
                transaction
            ).Using(
                command =>
                {
                    if (parameters != null)
                    {
                        parameters(command.Parameters, DatabaseParameterPhase.Fetch);
                    }
                    var result = command.ExecuteNonQuery();
                    if (parameters != null)
                    {
                        parameters(command.Parameters, DatabaseParameterPhase.Store);
                    }
                    return result;
                },
                () => transaction == null
            );
        }

        public T ExecuteScalar<T>(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return this.CreateCommand(
                query,
                DatabaseCommandFlags.None,
                transaction
            ).Using(
                command =>
                {
                    if (parameters != null)
                    {
                        parameters(command.Parameters, DatabaseParameterPhase.Fetch);
                    }
                    var result = Converter.ChangeType<T>(command.ExecuteScalar());
                    if (parameters != null)
                    {
                        parameters(command.Parameters, DatabaseParameterPhase.Store);
                    }
                    return result;
                },
                () => transaction == null
            );
        }

        [Obsolete]
        public IEnumerable<T> ExecuteEnumerator<T>(ITableConfig table, IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            using (var reader = this.ExecuteReader(query, parameters, transaction))
            {
                var mapper = new EntityMapper(table);
                var visitor = new EntityCompoundEnumeratorVisitor();
                var enumerable = new EntityCompoundEnumerator(this, table, mapper, visitor);
                var buffer = new EntityEnumeratorBuffer(this);
                var sink = new EntityEnumeratorSink(table);
                foreach (var element in enumerable.AsEnumerable<T>(buffer, sink, reader))
                {
                    yield return element;
                }
            }
        }

        [Obsolete]
        public IAsyncEnumerator<T> ExecuteAsyncEnumerator<T>(ITableConfig table, IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            var reader = this.ExecuteReader(query, parameters, transaction);
            var mapper = new EntityMapper(table);
            var visitor = new EntityCompoundEnumeratorVisitor();
            var enumerable = new EntityCompoundEnumerator(this, table, mapper, visitor);
            var buffer = new EntityEnumeratorBuffer(this);
            var sink = new EntityEnumeratorSink(table);
            return enumerable.AsEnumerableAsync<T>(buffer, sink, reader, true);
        }

        public IDatabaseReader ExecuteReader(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            var command = this.CreateCommand(query, DatabaseCommandFlags.None, transaction);
            if (parameters != null)
            {
                parameters(command.Parameters, DatabaseParameterPhase.Fetch);
            }
            var result = new DatabaseReader(command.Command, transaction == null);
            if (parameters != null)
            {
                parameters(command.Parameters, DatabaseParameterPhase.Store);
            }
            return result;
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

    public partial class Database
    {
        public Task<int> ExecuteAsync(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            if (string.IsNullOrEmpty(query.CommandText))
            {
                throw new InvalidOperationException("The query is empty.");
            }
            return this.CreateCommand(
                query,
                DatabaseCommandFlags.None,
                transaction
            ).Using(
                async command =>
                {
                    if (parameters != null)
                    {
                        parameters(command.Parameters, DatabaseParameterPhase.Fetch);
                    }
                    var result = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    if (parameters != null)
                    {
                        parameters(command.Parameters, DatabaseParameterPhase.Store);
                    }
                    return result;
                },
                () => transaction == null
            );
        }

        public Task<T> ExecuteScalarAsync<T>(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return this.CreateCommand(
                query,
                DatabaseCommandFlags.None,
                transaction
            ).Using(
                async command =>
                {
                    if (parameters != null)
                    {
                        parameters(command.Parameters, DatabaseParameterPhase.Fetch);
                    }
                    var result = Converter.ChangeType<T>(await command.ExecuteScalarAsync().ConfigureAwait(false));
                    if (parameters != null)
                    {
                        parameters(command.Parameters, DatabaseParameterPhase.Store);
                    }
                    return result;
                },
                () => transaction == null
            );
        }

        public Task<IEnumerable<T>> ExecuteEnumeratorAsync<T>(ITableConfig table, IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            throw new NotImplementedException();
        }
    }
}
