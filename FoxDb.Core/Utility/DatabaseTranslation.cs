using FoxDb.Interfaces;
using System.Data;

namespace FoxDb
{
    public class DatabaseTranslation : IDatabaseTranslation
    {
        public DatabaseTranslation(IDatabase database)
        {
            this.Database = database;
        }

        public IDatabase Database { get; private set; }

        public virtual DbType GetLocalType(DbType type)
        {
            return type;
        }

        public virtual DbType GetRemoteType(DbType type)
        {
            return type;
        }

        public virtual object GetLocalValue(DbType type, object value)
        {
            return value;
        }

        public virtual object GetRemoteValue(DbType type, object value)
        {
            return value;
        }
    }
}
