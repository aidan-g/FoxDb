#pragma warning disable 612, 618
using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class EntityEnumeratorBuffer : IEntityEnumeratorBuffer
    {
        private EntityEnumeratorBuffer()
        {
            this.Factories = new Dictionary<ITableConfig, IEntityFactory>();
            this.Buffer = new Dictionary<ITableConfig, object>();
        }

        public EntityEnumeratorBuffer(IDatabase database)
            : this()
        {
            this.Database = database;
        }

        public IDictionary<ITableConfig, IEntityFactory> Factories { get; private set; }

        public IDictionary<ITableConfig, object> Buffer { get; private set; }

        public IDatabase Database { get; private set; }

        public IDatabaseReaderRecord Record { get; set; }

        protected virtual IEntityFactory GetFactory(ITableConfig table)
        {
            var factory = default(IEntityFactory);
            if (!this.Factories.TryGetValue(table, out factory))
            {
                var initializer = new EntityInitializer(table);
                var populator = new EntityPopulator(this.Database, table);
                factory = new EntityFactory(table, initializer, populator);
                this.Factories.Add(table, factory);
            }
            return factory;
        }

        public void Update(IDatabaseReaderRecord record)
        {
            this.Record = record;
        }

        public bool Exists(ITableConfig table)
        {
            var item = default(object);
            return this.Buffer.TryGetValue(table, out item) && item != null;
        }

        public object Create(ITableConfig table)
        {
            var item = this.GetFactory(table).Create(this.Record);
            return this.Buffer[table] = item;
        }

        public object Get(ITableConfig table)
        {
            var item = default(object);
            if (this.Buffer.TryGetValue(table, out item))
            {
                return item;
            }
            return null;
        }

        protected virtual object Key(ITableConfig table)
        {
            var value = default(object);
            if (this.Record.TryGetValue(table.PrimaryKey, out value))
            {
                return this.Database.Translation.GetLocalValue(
                    table.PrimaryKey.ColumnType.Type,
                    value
                );
            }
            return null;
        }

        public bool HasKey(ITableConfig table)
        {
            var key = default(object);
            return this.HasKey(table, out key);
        }

        public bool HasKey(ITableConfig table, out object key)
        {
            key = this.Key(table);
            if (EntityKey.IsKey(table.PrimaryKey, key))
            {
                return true;
            }
            key = null;
            return false;
        }

        public bool KeyChanged(ITableConfig table)
        {
            if (!this.Exists(table))
            {
                return false;
            }
            var key = default(object);
            if (!this.HasKey(table, out key))
            {
                return true;
            }
            var item = this.Get(table);
            if (!EqualityComparerFactory.Instance.Create(table.PrimaryKey.Property.PropertyType).Equals(EntityKey.GetKey(table, item), key))
            {
                return true;
            }
            return false;
        }

        public void Remove(ITableConfig table)
        {
            this.Buffer[table] = null;
        }
    }
}
