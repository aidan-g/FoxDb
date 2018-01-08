using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class Config : IConfig
    {
        private Config()
        {
            this.Members = new DynamicMethod(this.GetType());
            this.Tables = new Dictionary<TableKey, ITableConfig>();
        }

        public Config(IDatabase database) : this()
        {
            this.Database = database;
        }

        protected DynamicMethod Members { get; private set; }

        protected virtual IDictionary<TableKey, ITableConfig> Tables { get; private set; }

        public IDatabase Database { get; private set; }

        public ITableConfig Table(Type tableType)
        {
            var table = this.Members.Invoke(this, "Table", tableType);
            return (ITableConfig)table;
        }

        public ITableConfig<T> Table<T>()
        {
            var key = new TableKey(typeof(T));
            if (!this.Tables.ContainsKey(key))
            {
                this.Tables[key] = this.CreateTable<T>();
            }
            return this.Tables[key] as ITableConfig<T>;
        }

        protected virtual ITableConfig<T> CreateTable<T>()
        {
            return Factories.Table.Create<T>(this);
        }

        public ITableConfig<T1, T2> Table<T1, T2>()
        {
            var key = new TableKey(typeof(T1), typeof(T2));
            if (!this.Tables.ContainsKey(key))
            {
                this.Tables[key] = this.CreateTable<T1, T2>();
            }
            return this.Tables[key] as ITableConfig<T1, T2>;
        }

        protected virtual ITableConfig<T1, T2> CreateTable<T1, T2>()
        {
            return Factories.Table.Create<T1, T2>(this);
        }

        protected class TableKey : IEquatable<TableKey>
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
