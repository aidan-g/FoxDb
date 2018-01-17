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

        public ITableConfig GetTable(ITableSelector selector)
        {
            var table = default(ITableConfig);
            if (!this.Tables.TryGetValue(this.GetKey(selector), out table))
            {
                return default(ITableConfig);
            }
            return table;
        }

        public ITableConfig CreateTable(ITableSelector selector)
        {
            var table = Factories.Table.Create(this, selector);
            if (!TableValidator.Validate(table))
            {
                throw new InvalidOperationException("Table configuration is not valid.");
            }
            this.Tables[this.GetKey(selector)] = table;
            this.Configure(table);
            return table;
        }

        public bool TryCreateTable(ITableSelector selector, out ITableConfig table)
        {
            table = Factories.Table.Create(this, selector);
            if (!TableValidator.Validate(table))
            {
                return false;
            }
            this.Tables[this.GetKey(selector)] = table;
            this.Configure(table);
            return true;
        }

        protected virtual void Configure(ITableConfig table)
        {
            if (table.Flags.HasFlag(TableFlags.AutoColumns))
            {
                table.AutoColumns();
            }
            if (table.Flags.HasFlag(TableFlags.AutoRelations))
            {
                table.AutoRelations();
            }
        }

        protected virtual TableKey GetKey(ITableSelector selector)
        {
            switch (selector.Type)
            {
                case TableSelectorType.TableType:
                    return new TableKey(selector.TableType);
                case TableSelectorType.Mapping:
                    return new TableKey(selector.LeftTable.TableType, selector.RightTable.TableType);
            }
            throw new NotImplementedException();
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
                unchecked
                {
                    foreach (var type in this.Types)
                    {
                        hashCode += type.GetHashCode();
                    }
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
