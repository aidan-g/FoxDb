using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public static class EntityKey<T>
    {
        public static bool HasKey(IDatabase database, T item)
        {
            var table = database.Config.Table<T>();
            if (table.PrimaryKey == null)
            {
                throw new InvalidOperationException(string.Format("Table \"{0}\" does not have a valid primary key configuration.", table.TableName));
            }
            var key = table.PrimaryKey.Getter(item);
            if (key != null && !key.Equals(table.PrimaryKey.PropertyType.DefaultValue()))
            {
                return true;
            }
            return false;
        }

        public static void SetKey(IDatabase database, T item, object key)
        {
            var table = database.Config.Table<T>();
            if (table.PrimaryKey == null)
            {
                throw new InvalidOperationException(string.Format("Table \"{0}\" does not have a valid primary key configuration.", table.TableName));
            }
            table.PrimaryKey.Setter(item, key);
        }

    }
}
