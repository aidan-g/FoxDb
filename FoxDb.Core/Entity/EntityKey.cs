using FoxDb.Interfaces;
using System;
using System.Linq;

namespace FoxDb
{
    public static class EntityKey
    {
        public static bool IsKey(object key)
        {
            return key != null && !DBNull.Value.Equals(key);
        }

        public static bool HasKey(ITableConfig table, object item)
        {
            if (table.PrimaryKey == null)
            {
                throw new InvalidOperationException(string.Format("Table \"{0}\" does not have a valid primary key configuration.", table.TableName));
            }
            var key = table.PrimaryKey.Getter(item);
            if (key != null && !key.Equals(table.PrimaryKey.Property.PropertyType.DefaultValue()))
            {
                return true;
            }
            return false;
        }

        public static object GetKey(ITableConfig table, object item)
        {
            if (table.PrimaryKey == null)
            {
                throw new InvalidOperationException(string.Format("Table \"{0}\" does not have a valid primary key configuration.", table.TableName));
            }
            return table.PrimaryKey.Getter(item);
        }

        public static void SetKey(ITableConfig table, object item, object key)
        {
            if (table.PrimaryKey == null)
            {
                throw new InvalidOperationException(string.Format("Table \"{0}\" does not have a valid primary key configuration.", table.TableName));
            }
            table.PrimaryKey.Setter(item, key);
        }

        public static bool KeyEquals(ITableConfig table, object item, object key)
        {
            //I don't really like using dynamic but it seems to work here.
            //The problem is that either key could be a) boxed b) of differing types.
            //The alternative is to determine the "widest" type and use Convert.ChangeType and Equals to determine equality.
            return (dynamic)GetKey(table, item) == (dynamic)key;
        }
    }
}
