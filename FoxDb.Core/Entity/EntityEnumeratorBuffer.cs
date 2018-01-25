﻿using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class EntityEnumeratorBuffer : IEntityEnumeratorBuffer
    {
        public EntityEnumeratorBuffer()
        {
            this.Factories = new Dictionary<ITableConfig, IEntityFactory>();
            this.Buffer = new Dictionary<ITableConfig, object>();
        }

        public IDictionary<ITableConfig, IEntityFactory> Factories { get; private set; }

        public IDictionary<ITableConfig, object> Buffer { get; private set; }

        public IDatabaseReaderRecord Record { get; set; }

        protected virtual IEntityFactory GetFactory(ITableConfig table)
        {
            var factory = default(IEntityFactory);
            if (!this.Factories.TryGetValue(table, out factory))
            {
                var initializer = new EntityInitializer(table);
                var populator = new EntityPopulator(table);
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
            if (table.PrimaryKey != null)
            {
                if (this.Record.Contains(table.PrimaryKey.Identifier))
                {
                    return this.Record[table.PrimaryKey.Identifier];
                }
                if (this.Record.Contains(table.PrimaryKey.ColumnName))
                {
                    return this.Record[table.PrimaryKey.ColumnName];
                }
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
            if (EntityKey.IsKey(key))
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
            if (!EntityKey.KeyEquals(table, item, key))
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
