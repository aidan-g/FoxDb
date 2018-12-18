using FoxDb.Interfaces;
using System.Data;

namespace FoxDb.Utility
{
    public class NullTransactionSource : Disposable, ITransactionSource
    {
        public NullTransactionSource(IDatabase database)
        {
            this.Database = database;
            this.CommandCache = new DatabaseCommandCache(database);
        }

        public IDatabase Database { get; private set; }

        public IDatabaseCommandCache CommandCache { get; private set; }

        public bool HasTransaction
        {
            get
            {
                return false;
            }
        }

        public void Bind(IDbCommand command)
        {
            //Nothing to do.
        }

        public void Commit()
        {
            //Nothing to do.
        }

        public void Rollback()
        {
            //Nothing to do.
        }
    }
}
