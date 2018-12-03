using FoxDb.Interfaces;
using System;
using System.Collections.Concurrent;

namespace FoxDb
{
    public class Config : IConfig
    {
        private Config()
        {
            this.Members = new DynamicMethod<Config>();
            this.Reset();
        }

        public Config(IDatabase database)
            : this()
        {
            this.Database = database;
        }

        protected DynamicMethod<Config> Members { get; private set; }

        protected virtual ConcurrentDictionary<string, ITableConfig> Tables { get; private set; }

        public IDatabase Database { get; private set; }

        public IConfig Transient
        {
            get
            {
                return new Config(this.Database);
            }
        }

        public ITableConfig GetTable(ITableSelector selector)
        {
            var existing = default(ITableConfig);
            var table = Factories.Table.Create(this, selector);
            if (!this.Tables.TryGetValue(table.Identifier, out existing) || !TableComparer.TableConfig.Equals(table, existing))
            {
                return default(ITableConfig);
            }
            return existing;
        }

        public ITableConfig CreateTable(ITableSelector selector)
        {
            var table = Factories.Table.Create(this, selector);
            if (!TableValidator.Validate(table))
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
            if (!TableValidator.Validate(table))
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
            if (table.Flags.HasFlag(TableFlags.AutoIndexes))
            {
                table.AutoIndexes();
            }
            if (table.Flags.HasFlag(TableFlags.AutoRelations))
            {
                table.AutoRelations();
            }
            if (typeof(IEntityConfiguration).IsAssignableFrom(table.TableType))
            {
                this.ConfigureEntity(table);
            }
        }

        protected virtual void ConfigureEntity(ITableConfig table)
        {
            var initializer = new EntityInitializer(table);
            var factory = new EntityFactory(table, initializer);
            var entity = factory.Create() as IEntityConfiguration;
            if (entity == null)
            {
                return;
            }
            entity.Configure(this, table);
        }

        public void Reset()
        {
            this.Tables = new ConcurrentDictionary<string, ITableConfig>(StringComparer.OrdinalIgnoreCase);
        }

        public void CopyTo(IConfig config)
        {
            if (!(config is Config))
            {
                throw new NotImplementedException();
            }
            (config as Config).Tables = this.Tables;
        }
    }
}
