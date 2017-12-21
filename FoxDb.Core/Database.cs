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
            this.Connection = this.Provider.CreateConnection();
        }

        public IConfig Config { get; private set; }

        public IProvider Provider { get; private set; }

        public IDbConnection Connection { get; private set; }

        public IDatabaseSet<T> Set<T>(IDbTransaction transaction = null) where T : IPersistable
        {
            return this.Query<T>(this.Provider.QueryFactory.Select<T>(), transaction);
        }

        public IDatabaseSet<T> Query<T>(IDatabaseQuery query, IDbTransaction transaction = null) where T : IPersistable
        {
            return this.Query<T>(query, null, transaction);
        }

        public IDatabaseSet<T> Query<T>(IDatabaseQuery query, DatabaseParameterHandler handler, IDbTransaction transaction = null) where T : IPersistable
        {
            return new DatabaseSet<T>(this, query, handler, transaction);
        }
    }
}
