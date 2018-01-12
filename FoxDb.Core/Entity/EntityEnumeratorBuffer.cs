using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class EntityEnumeratorBuffer : IEntityEnumeratorBuffer
    {
        private EntityEnumeratorBuffer()
        {
            this.Factories = new Dictionary<Type, IEntityFactory>();
            this.Buffer = new Dictionary<Type, object>();
        }

        public EntityEnumeratorBuffer(IDatabaseSet set) : this()
        {
            this.Set = set;
        }

        public IDictionary<Type, IEntityFactory> Factories { get; private set; }

        public IDictionary<Type, object> Buffer { get; private set; }

        public IDatabaseSet Set { get; private set; }

        public IDatabaseReaderRecord Record { get; set; }

        protected virtual IEntityFactory<T> GetFactory<T>(ITableConfig table)
        {
            var factory = default(IEntityFactory);
            if (!this.Factories.TryGetValue(typeof(T), out factory))
            {
                var initializer = new EntityInitializer<T>(table, this.Set.Mapper);
                var populator = new EntityPopulator<T>(table, this.Set.Mapper);
                factory = new EntityFactory<T>(initializer, populator);
                this.Factories.Add(typeof(T), factory);
            }
            return (IEntityFactory<T>)factory;
        }

        public void Update(IDatabaseReaderRecord record)
        {
            this.Record = record;
        }

        public bool Exists<T>()
        {
            return this.Buffer.ContainsKey(typeof(T));
        }

        public T Create<T>(ITableConfig table)
        {
            var item = this.GetFactory<T>(table).Create(this.Record);
            this.Buffer.Add(typeof(T), item);
            return item;
        }

        public T Get<T>()
        {
            var item = default(object);
            if (this.Buffer.TryGetValue(typeof(T), out item))
            {
                return (T)item;
            }
            return default(T);
        }

        protected virtual object Key(ITableConfig table)
        {
            if (table.PrimaryKey == null)
            {
                return null;
            }
            if (!this.Record.Contains(table.PrimaryKey.Identifier))
            {
                return null;
            }
            return this.Record[table.PrimaryKey.Identifier];
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

        public bool KeyChanged<T>(ITableConfig table)
        {
            if (!this.Exists<T>())
            {
                return false;
            }
            var key = default(object);
            if (!this.HasKey(table, out key))
            {
                return true;
            }
            var item = this.Get<T>();
            if (!EntityKey.KeyEquals(table, item, key))
            {
                return true;
            }
            return false;
        }

        public void Remove<T>()
        {
            this.Buffer.Remove(typeof(T));
        }
    }
}
