using FoxDb.Interfaces;
using System;
using System.Linq;

namespace FoxDb
{
    public static class EntityKey
    {
        /// <summary>
        /// The most likely key types.
        /// </summary>
        private static readonly Type[] IntegralTypes = new[]
        {
            typeof(long),
            typeof(int),
            typeof(byte),
            typeof(ulong),
            typeof(uint)
        };

        /// <summary>
        /// Unique identifier types.
        /// </summary>
        public static readonly Type[] IdentifierTypes = new[]
        {
            typeof(Guid)
        };

        /// <summary>
        /// Other key types.
        /// </summary>
        private static readonly Type[] TextTypes = new[]
        {
            typeof(string)
        };

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
            var type = table.PrimaryKey.Property.PropertyType;
            if (IntegralTypes.Contains(type))
            {
                var key1 = Converter.ChangeType<long>(GetKey(table, item));
                var key2 = Converter.ChangeType<long>(key);
                return key1 == key2;
            }
            if (IdentifierTypes.Contains(type))
            {
                var key1 = Converter.ChangeType<Guid>(GetKey(table, item));
                var key2 = Converter.ChangeType<Guid>(key);
                return key1 == key2;
            }
            if (TextTypes.Contains(type))
            {
                var key1 = Converter.ChangeType<string>(GetKey(table, item));
                var key2 = Converter.ChangeType<string>(key);
                return string.Equals(key1, key2);
            }
            throw new NotImplementedException();
        }
    }
}
