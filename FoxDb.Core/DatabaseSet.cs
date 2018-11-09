﻿#pragma warning disable 612, 618
using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public partial class DatabaseSet<T> : IDatabaseSet<T>
    {
        private DatabaseSet()
        {
            this.StateDetector = new Lazy<IEntityStateDetector>(() =>
            {
                var stateDetector = new EntityStateDetector(this.Database, this.Table, this.Source.Composer, this.Transaction);
                return stateDetector;
            });
            this.Persister = new Lazy<IEntityPersister>(() =>
            {
                var mapper = new EntityMapper(this.Table);
                var visitor = new EntityCompoundPersisterVisitor(this.Database, this.Transaction);
                var persister = new EntityCompoundPersister(this.Database, this.Table, mapper, visitor, this.Transaction);
                return persister;
            });
            this.Enumerator = new Lazy<IEntityEnumerator>(() =>
            {
                var mapper = new EntityMapper(this.Table);
                var visitor = new EntityCompoundEnumeratorVisitor();
                var enumerator = new EntityCompoundEnumerator(this.Database, this.Table, mapper, visitor);
                return enumerator;
            });
        }

        public DatabaseSet(IDatabaseQuerySource source)
            : this()
        {
            this.Source = source;
        }

        public Lazy<IEntityStateDetector> StateDetector { get; private set; }

        public Lazy<IEntityPersister> Persister { get; private set; }

        public Lazy<IEntityEnumerator> Enumerator { get; private set; }

        public Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        public IDatabaseQuerySource Source { get; private set; }

        public IDatabase Database
        {
            get
            {
                return this.Source.Database;
            }
        }

        public ITableConfig Table
        {
            get
            {
                return this.Source.Composer.Table;
            }
        }

        public DatabaseParameterHandler Parameters
        {
            get
            {
                return this.Source.Parameters;
            }
            set
            {
                this.Source.Parameters = value;
            }
        }

        public ITransactionSource Transaction
        {
            get
            {
                return this.Source.Transaction;
            }
        }
    }

    public partial class DatabaseSet<T>
    {
        object IDatabaseSet.Create()
        {
            var initializer = new EntityInitializer(this.Table);
            var factory = new EntityFactory(this.Table, initializer);
            return factory.Create();
        }

        object IDatabaseSet.Find(params object[] keys)
        {
            var fetch = this.Source.Composer.Fetch.With(query => query.Filter.AddColumns(this.Table.PrimaryKeys));
            var parameters = new PrimaryKeysParameterHandlerStrategy(this.Table, keys);
            using (var reader = this.Database.ExecuteReader(fetch, parameters.Handler, this.Transaction))
            {
                var buffer = new EntityEnumeratorBuffer(this.Database);
                var sink = new EntityEnumeratorSink(this.Table);
                return this.Enumerator.Value.AsEnumerable(buffer, sink, reader).OfType<object>().FirstOrDefault();
            }
        }

        object IDatabaseSet.AddOrUpdate(object item)
        {
            var set = (IDatabaseSet)this;
            var persisted = set.Find(EntityKey.GetKey(this.Table, item));
            if (persisted == null)
            {
                this.Persister.Value.Add(item);
            }
            else
            {
                this.Persister.Value.Update(persisted, item);
            }
            return item;
        }

        IEnumerable<object> IDatabaseSet.AddOrUpdate(IEnumerable<object> items)
        {
            var set = (IDatabaseSet)this;
            foreach (var item in items)
            {
                set.AddOrUpdate(item);
            }
            return items;
        }

        IEnumerable<object> IDatabaseSet.Remove(IEnumerable<object> items)
        {
            foreach (var item in items)
            {
                this.Persister.Value.Delete(item);
            }
            return items;
        }
    }

    public partial class DatabaseSet<T>
    {
        T IDatabaseSet<T>.Create()
        {
            var initializer = new EntityInitializer(this.Table);
            var factory = new EntityFactory(this.Table, initializer);
            return (T)factory.Create();
        }

        T IDatabaseSet<T>.Find(params object[] keys)
        {
            var fetch = this.Source.Composer.Fetch.With(query => query.Filter.AddColumns(this.Table.PrimaryKeys));
            var parameters = new PrimaryKeysParameterHandlerStrategy(this.Table, keys);
            using (var reader = this.Database.ExecuteReader(fetch, parameters.Handler, this.Transaction))
            {
                var buffer = new EntityEnumeratorBuffer(this.Database);
                var sink = new EntityEnumeratorSink(this.Table);
                return this.Enumerator.Value.AsEnumerable<T>(buffer, sink, reader).FirstOrDefault();
            }
        }

        T IDatabaseSet<T>.AddOrUpdate(T item)
        {
            var set = (IDatabaseSet<T>)this;
            var persisted = set.Find(EntityKey.GetKey(this.Table, item));
            if (persisted == null)
            {
                this.Persister.Value.Add(item);
            }
            else
            {
                this.Persister.Value.Update(persisted, item);
            }
            return item;
        }

        IEnumerable<T> IDatabaseSet<T>.AddOrUpdate(IEnumerable<T> items)
        {
            var set = (IDatabaseSet<T>)this;
            foreach (var item in items)
            {
                set.AddOrUpdate(item);
            }
            return items;
        }

        IEnumerable<T> IDatabaseSet<T>.Remove(IEnumerable<T> items)
        {
            var set = (IDatabaseSet<T>)this;
            foreach (var item in items)
            {
                this.Persister.Value.Delete(item);
            }
            return items;
        }
    }

    public partial class DatabaseSet<T>
    {
        int ICollection<T>.Count
        {
            get
            {
                var query = this.Database.QueryFactory.Count(this.Table, this.Source.Fetch);
                return this.Database.ExecuteScalar<int>(query, this.Parameters, this.Transaction);
            }
        }

        void ICollection<T>.Clear()
        {
            var set = (IDatabaseSet<T>)this;
            set.Remove(this);
        }

        bool ICollection<T>.Contains(T item)
        {
            switch (this.StateDetector.Value.GetState(item))
            {
                case EntityState.Exists:
                    return true;
            }
            return false;
        }

        void ICollection<T>.CopyTo(T[] target, int index)
        {
            foreach (var element in this)
            {
                if (index >= target.Length)
                {
                    throw new IndexOutOfRangeException();
                }
                target[index] = element;
                index++;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        void ICollection<T>.Add(T item)
        {
            var set = (IDatabaseSet<T>)this;
            set.AddOrUpdate(item);
        }

        bool ICollection<T>.Remove(T item)
        {
            var set = (IDatabaseSet<T>)this;
            if (!set.Contains(item))
            {
                return false;
            }
            set.Remove(new[] { item });
            return true;
        }
    }

    public partial class DatabaseSet<T>
    {
        IDatabaseQueryComposer IDatabaseQuerySource.Composer
        {
            get
            {
                return this.Source.Composer;
            }
        }

        IQueryGraphBuilder IDatabaseQuerySource.Fetch
        {
            get
            {
                return this.Source.Fetch;
            }
            set
            {
                this.Source.Fetch = value;
            }
        }

        IQueryGraphBuilder IDatabaseQuerySource.Add
        {
            get
            {
                return this.Source.Add;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        IQueryGraphBuilder IDatabaseQuerySource.Update
        {
            get
            {
                return this.Source.Update;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        IQueryGraphBuilder IDatabaseQuerySource.Delete
        {
            get
            {
                return this.Source.Delete;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        void IDatabaseQuerySource.Reset()
        {
            this.Source.Reset();
        }

        IDatabaseQuerySource ICloneable<IDatabaseQuerySource>.Clone()
        {
            return this.Source.Clone();
        }
    }

    public partial class DatabaseSet<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            using (var reader = this.Database.ExecuteReader(this.Source.Fetch, this.Parameters, this.Transaction))
            {
                var buffer = new EntityEnumeratorBuffer(this.Database);
                var sink = new EntityEnumeratorSink(this.Table);
                foreach (var element in this.Enumerator.Value.AsEnumerable<T>(buffer, sink, reader))
                {
                    yield return element;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
