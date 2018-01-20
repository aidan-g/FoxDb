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
            var persister = new EntityPersister<T>(this);
            foreach (var item in items)
            {
                persister.AddOrUpdate(item);
            }
            return items;
        }

        public void Clear()
        {
            this.Delete(this);
        }

        public T Delete(T item)
        {
            this.Delete(new[] { item });
            return item;
        }

        public IEnumerable<T> Delete(IEnumerable<T> items)
        {
            var persister = new EntityPersister<T>(this);
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
                set.Fetch = this.Source.Composer.Query.With(query => query.Filter.AddColumns(this.Table.PrimaryKeys));
                set.Parameters = new PrimaryKeysParameterHandlerStrategy<T>(this.Database, id).Handler;
            }).FirstOrDefault();
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
                var enumerable = new EntityCompoundEnumerator(this, reader);
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
                return this.Source.Table;
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

        public IEntityMapper Mapper
        {
            get
            {
                return this.Source.Mapper;
            }
        }

        public IEntityRelationQueryComposer Composer
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
                this.Source.Add = value;
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
                this.Source.Update = value;
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
                this.Source.Delete = value;
            }
        }

        void IDatabaseQuerySource.Reset()
        {
            this.Source.Reset();
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
                return true;
            }
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotImplementedException();
        }

        bool ICollection<T>.Contains(T item)
        {
            throw new NotImplementedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
