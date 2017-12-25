using FoxDb.Interfaces;
using System.Data;

namespace FoxDb
{
    public class DatabaseQuerySource<T> : IDatabaseQuerySource<T>
    {
        public DatabaseQuerySource(IDatabase database, IDbTransaction transaction = null)
        {
            this.Database = database;
            this.Select = database.QueryFactory.Select<T>();
            this.Insert = database.QueryFactory.Insert<T>();
            this.Update = database.QueryFactory.Update<T>();
            this.Delete = database.QueryFactory.Delete<T>();
            this.Transaction = transaction;
        }

        public IDatabase Database { get; private set; }

        public IDatabaseQuery Select { get; set; }

        public IDatabaseQuery Insert { get; set; }

        public IDatabaseQuery Update { get; set; }

        public IDatabaseQuery Delete { get; set; }

        public DatabaseParameterHandler Parameters { get; set; }

        public IDbTransaction Transaction { get; set; }
    }
}
