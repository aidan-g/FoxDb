using FoxDb.Interfaces;
using System.Data;

namespace FoxDb
{
    public class Database : IDatabase
    {
        private Database()
        {
            this.Config = new Config();
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

        public IDatabaseSet<T> Set<T>(IDbTransaction transaction = null) where T : IPersistable
        {
            return this.Query<T>(new DatabaseQuerySource<T>(this, transaction));
        }

        public IDatabaseSet<T> Query<T>(IDatabaseQuerySource<T> source) where T : IPersistable
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

        public T Execute<T>(IDatabaseQuery query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null)
        {
            using (var command = this.Connection.CreateCommand(query, parameters, transaction))
            {
                return Converter.ChangeType<T>(command.ExecuteScalar());
            }
        }

        public IDatabaseReader ExecuteReader(IDatabaseQuery query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null)
        {
            var command = this.Connection.CreateCommand(query, parameters, transaction);
            return new DatabaseReader(command.ExecuteReader());
        }
    }
}
