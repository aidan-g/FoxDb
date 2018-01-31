using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class DatabaseSet<T> : IDatabaseSet<T>
    {
        public DatabaseSet(IDatabaseQuerySource source)
        {
            this.Source = source;
        }

        public Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        public IDatabaseQuerySource Source { get; private set; }

        public int Count
        {
            get
            {
                var query = this.Database.QueryFactory.Count(this.Table, this.Source.Fetch);
                return this.Database.ExecuteScalar<int>(query, this.Parameters, this.Transaction);
            }
        }

        public T AddOrUpdate(T item)
        {
            this.AddOrUpdate(new[] { item });
            return item;
        }

        public IEnumerable<T> AddOrUpdate(IEnumerable<T> items)
        {
            var mapper = new EntityMapper(this.Table);
            var persister = new EntityCompoundPersister(this.Database, this.Table, mapper, this.Transaction);
            foreach (var item in items)
            {
                persister.AddOrUpdate(item);
            }
            return items;
        }

        public void Clear()
        {
            this.Remove(this);
        }

        public T Remove(T item)
        {
            this.Remove(new[] { item });
            return item;
        }

        public IEnumerable<T> Remove(IEnumerable<T> items)
        {
            var mapper = new EntityMapper(this.Table);
            var persister = new EntityCompoundPersister(this.Database, this.Table, mapper, this.Transaction);
            foreach (var item in items)
            {
                persister.Delete(item);
            }
            return items;
        }

        public T Create()
        {
            var initializer = new EntityInitializer(this.Table);
            var factory = new EntityFactory(this.Table, initializer);
            return (T)factory.Create();
        }

        public T Find(object id)
        {
            return this.Database.Set<T>(this.Table).With(set =>
            {
                set.Fetch = this.Source.Composer.Fetch.With(query => query.Filter.AddColumns(this.Table.PrimaryKeys));
                set.Parameters = new PrimaryKeysParameterHandlerStrategy(this.Table, new[] { id }).Handler;
            }).FirstOrDefault();
        }

        public bool Contains(T item)
        {
            return this.Find(EntityKey.GetKey(this.Table, item)) != null;
        }

        public void CopyTo(T[] target, int index)
        {
            foreach (var element in this)
            {
                target[index++] = element;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            using (var reader = this.Database.ExecuteReader(this.Source.Fetch, this.Parameters, this.Transaction))
            {
                var mapper = new EntityMapper(this.Table);
                var enumerable = new EntityCompoundEnumerator(this.Table, mapper, reader);
                foreach (var element in enumerable.AsEnumerable<T>())
                {
                    yield return element;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #region IDatabaseQuerySource

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

        #endregion

        #region ICollection<T>

        /*
         * We only implement these things so LINQ uses the Count property instead of iterating.
         */

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        void ICollection<T>.Add(T item)
        {
            item = this.AddOrUpdate(item);
        }

        bool ICollection<T>.Remove(T item)
        {
            if (!this.Contains(item))
            {
                return false;
            }
            item = this.Remove(item);
            return true;
        }

        #endregion
    }
}
