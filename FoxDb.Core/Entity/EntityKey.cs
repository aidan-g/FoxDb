#pragma warning disable 612, 618
using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public static class EntityKey
    {
        public static bool IsKey(IColumnConfig column, object key)
        {
            return key != null && !DBNull.Value.Equals(key) && !key.Equals(column.DefaultValue);
        }

        [Obsolete("This routine assumes a single primary key.")]
        public static bool HasKey(ITableConfig table, object item)
        {
            var key = default(object);
            return HasKey(table, item, out key);
        }

        [Obsolete("This routine assumes a single primary key.")]
        public static bool HasKey(ITableConfig table, object item, out object key)
        {
            if (table.PrimaryKey == null)
            {
                throw new InvalidOperationException(string.Format("Table \"{0}\" does not have a valid primary key configuration.", table.TableName));
            }
            return IsKey(table.PrimaryKey, key = table.PrimaryKey.Getter(item));
        }

        [Obsolete("This routine assumes a single primary key.")]
        public static object GetKey(ITableConfig table, object item)
        {
            if (table.PrimaryKey == null)
            {
                throw new InvalidOperationException(string.Format("Table \"{0}\" does not have a valid primary key configuration.", table.TableName));
            }
            return table.PrimaryKey.Getter(item);
        }

        [Obsolete("This routine assumes a single primary key.")]
        public static void SetKey(ITableConfig table, object item, object key)
        {
            if (table.PrimaryKey == null)
            {
                throw new InvalidOperationException(string.Format("Table \"{0}\" does not have a valid primary key configuration.", table.TableName));
            }
            table.PrimaryKey.Setter(item, key);
        }
    }
}
