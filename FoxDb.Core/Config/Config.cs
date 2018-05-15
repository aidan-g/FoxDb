using FoxDb.Interfaces;
using System;
using System.Collections.Concurrent;

namespace FoxDb
{
    public class Config : IConfig
    {
        private Config()
        {
            this.Members = new DynamicMethod(this.GetType());
            this.Tables = new ConcurrentDictionary<string, ITableConfig>(StringComparer.OrdinalIgnoreCase);
        }

        public Config(IDatabase database)
            : this()
        {
            this.Database = database;
        }

        protected DynamicMethod Members { get; private set; }

        protected virtual ConcurrentDictionary<string, ITableConfig> Tables { get; private set; }

        public IDatabase Database { get; private set; }

        public ITableConfig GetTable(ITableSelector selector)
        {
            var existing = default(ITableConfig);
            var table = Factories.Table.Create(this, selector);
            if (!this.Tables.TryGetValue(table.Identifier, out existing) || !table.Equals(existing))
            {
                return default(ITableConfig);
            }
            return existing;
        }

        public ITableConfig CreateTable(ITableSelector selector)
        {
            var table = Factories.Table.Create(this, selector);
            if (selector.Flags.HasFlag(TableFlags.ValidateSchema) && !TableValidator.Validate(table))
            {
                throw new InvalidOperationException(string.Format("Table has invalid configuration: {0}", table));
            }
            table = this.Tables.AddOrUpdate(table.Identifier, table);
            this.Configure(table);
            return table;
        }

        public bool TryCreateTable(ITableSelector selector, out ITableConfig table)
        {
            table = Factories.Table.Create(this, selector);
            if (selector.Flags.HasFlag(TableFlags.ValidateSchema) && !TableValidator.Validate(table))
            {
                return false;
            }
            table = this.Tables.AddOrUpdate(table.Identifier, table);
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

        public static IConfig Transient
        {
            get
            {
                return new Config();
            }
        }
    }
}
