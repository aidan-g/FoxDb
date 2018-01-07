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

        protected virtual IEntityFactory<T> GetFactory<T>()
        {
            var factory = default(IEntityFactory);
            if (!this.Factories.TryGetValue(typeof(T), out factory))
            {
                var table = this.Set.Database.Config.Table<T>();
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

        public T Create<T>()
        {
            var item = this.GetFactory<T>().Create(this.Record);
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

        protected virtual object Key<T>()
        {
            var table = this.Set.Database.Config.Table<T>();
            if (table.PrimaryKey == null)
            {
                return null;
            }
            var identifier = this.Set.Mapper.GetColumn(table.PrimaryKey).Identifier;
            if (!this.Record.Contains(identifier))
            {
                return null;
            }
            return this.Record[identifier];
        }

        public bool HasKey<T>()
        {
            var key = default(object);
            return this.HasKey<T>(out key);
        }

        protected virtual bool HasKey<T>(out object key)
        {
            key = this.Key<T>();
            if (EntityKey<T>.IsKey(this.Set.Database, key))
            {
                return true;
            }
            key = null;
            return false;
        }

        public bool KeyChanged<T>()
        {
            if (!this.Exists<T>())
            {
                return false;
            }
            var key = default(object);
            if (!this.HasKey<T>(out key))
            {
                return true;
            }
            var item = this.Get<T>();
            if (!EntityKey<T>.KeyEquals(this.Set.Database, item, key))
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
