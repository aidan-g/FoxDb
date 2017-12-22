using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class Config : IConfig
    {
        public Config()
        {
            this.Tables = new Dictionary<TableKey, ITableConfig>();
        }

        private Dictionary<TableKey, ITableConfig> Tables { get; set; }

        public ITableConfig<T> Table<T>() where T : IPersistable
        {
            var key = new TableKey(typeof(T));
            if (!this.Tables.ContainsKey(key))
            {
                var config = new TableConfig<T>();
                this.Tables.Add(key, config);
            }
            return this.Tables[key] as ITableConfig<T>;
        }

        public ITableConfig<T1, T2> Table<T1, T2>()
            where T1 : IPersistable
            where T2 : IPersistable
        {
            var key = new TableKey(typeof(T1), typeof(T2));
            if (!this.Tables.ContainsKey(key))
            {
                var config = new TableConfig<T1, T2>();
                this.Tables.Add(key, config);
            }
            return this.Tables[key] as ITableConfig<T1, T2>;
        }

        private class TableKey : IEquatable<TableKey>
        {
            public TableKey(params Type[] types)
            {
                this.Types = types;
            }

            public Type[] Types { get; private set; }

            public override int GetHashCode()
            {
                var hashCode = 0;
                foreach (var type in this.Types)
                {
                    unchecked { hashCode += type.GetHashCode(); }
                }
                return hashCode;
            }

            public override bool Equals(object other)
            {
                if (other is TableKey)
                {
                    return this.Equals(other as TableKey);
                }
                return base.Equals(other);
            }

            public bool Equals(TableKey other)
            {
                if (other == null)
                {
                    return false;
                }
                else if (this.Types.Length != other.Types.Length)
                {
                    return false;
                }
                for (var a = 0; a < this.Types.Length; a++)
                {
                    if (this.Types[a] != other.Types[a])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
