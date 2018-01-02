using FoxDb.Interfaces;
using System.Data;

namespace FoxDb
{
    public class Database : IDatabase
    {
        private Database()
        {
            this.Config = new Config(this);
        }

        public Database(IProvider provider) : this()
        {
            this.Provider = provider;
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

        private IDatabaseSchema _Schema { get; set; }

        public IDatabaseSchema Schema
        {
            get
            {
                if (this._Schema == null)
                {
                    this._Schema = this.Provider.CreateSchema(this);
                }
                return this._Schema;
            }
        }

        private IDatabaseQueryFactory _QueryFactory { get; set; }

        public IDatabaseQueryFactory QueryFactory
        {
            get
            {
                if (this._QueryFactory == null)
                {
                    this._QueryFactory = this.Provider.CreateQueryFactory(this);
                }
                return this._QueryFactory;
            }
        }

        public IDatabaseSet<T> Set<T>(IDbTransaction transaction = null)
        {
            return this.Set<T>(false, transaction);
        }

        public IDatabaseSet<T> Set<T>(bool includeRelations, IDbTransaction transaction = null)
        {
            return this.Query<T>(new DatabaseQuerySource<T>(this, includeRelations, transaction));
        }

        public IDatabaseSet<T> Query<T>(IDatabaseQuerySource source)
        {
            return new DatabaseSet<T>(source);
        }

        public void Execute(IDatabaseQuery query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null)
        {
            using (var command = this.Connection.CreateCommand(query, parameters, transaction))
            {
                command.ExecuteNonQuery();
            }
        }

        public void Execute(IQueryGraphBuilder query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null)
        {
            this.Execute(this.QueryFactory.Create(query), parameters, transaction);
        }

        public T Execute<T>(IDatabaseQuery query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null)
        {
            using (var command = this.Connection.CreateCommand(query, parameters, transaction))
            {
                return Converter.ChangeType<T>(command.ExecuteScalar());
            }
        }

        public T Execute<T>(IQueryGraphBuilder query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null)
        {
            return this.Execute<T>(this.QueryFactory.Create(query), parameters, transaction);
        }

        public IDatabaseReader ExecuteReader(IDatabaseQuery query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null)
        {
            var command = this.Connection.CreateCommand(query, parameters, transaction);
            return new DatabaseReader(command.ExecuteReader());
        }

        public IDatabaseReader ExecuteReader(IQueryGraphBuilder query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null)
        {
            return this.ExecuteReader(this.QueryFactory.Create(query), parameters, transaction);
        }
    }
}
