using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabaseTranslation
    {
        DbType GetLocalType(DbType type);

        DbType GetRemoteType(DbType type);

        object GetLocalValue(DbType type, object value);

        object GetRemoteValue(DbType type, object value);
    }
}
