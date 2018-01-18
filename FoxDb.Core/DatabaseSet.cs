using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace FoxDb
{
    public class DatabaseSet<T> : IDatabaseSet<T>
    {
        public DatabaseSet(ITableConfig table, IDatabaseQuerySource source)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            this.Table = table;
            this.Source = source;
        }

        public Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        public IDatabase Database
        {
            get
            {
                return this.Source.Database;
            }
        }

        public ITableConfig Table { get; private set; }

        public IEntityMapper Mapper
        {
            get
            {
                return this.Source.Mapper;
            }
        }

        public IEntityInitializer Initializer
        {
            get
            {
                return this.Source.Initializer;
            }
        }

        public IEntityPopulator Populator
        {
            get
            {
                return this.Source.Populator;
            }
        }

        public IEntityFactory Factory
        {
            get
            {
                return this.Source.Factory;
            }
        }

        public IDatabaseQuerySource Source { get; private set; }

        public DatabaseParameterHandler Parameters
        {
            get
            {
                return this.Source.Parameters;
            }
        }

        public IDbTransaction Transaction
        {
            get
            {
                return this.Source.Transaction;
            }
        }

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
            return (T)this.Factory.Create();
        }

        public T Find(object id)
        {
            var query = default(IQueryGraphBuilder);
            if (this.Source.Composer != null)
            {
                query = this.Source.Composer.Query;
            }
            else if (this.Source.Fetch != null)
            {
                query = this.Source.Fetch.Clone();
            }
            else
            {
                query = this.Database.QueryFactory.Fetch(this.Table);
            }
            query.Filter.AddColumns(this.Table.PrimaryKeys);
            var parameters = new PrimaryKeysParameterHandlerStrategy<T>(this.Database, id).Handler;
            var sequence = this.GetEnumerator(query, parameters);
            if (sequence.MoveNext())
            {
                return sequence.Current;
            }
            return default(T);
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
            if (!this.Source.CanRead)
            {
                throw new InvalidOperationException(string.Format("Query source cannot be read."));
            }
            return this.GetEnumerator(this.Source.Fetch, this.Parameters);
        }

        protected virtual IEnumerator<T> GetEnumerator(IQueryGraphBuilder query, DatabaseParameterHandler parameters)
        {
            using (var reader = this.Database.ExecuteReader(query, parameters, this.Transaction))
            {
                var enumerator = new EntityEnumerator();
                foreach (var item in enumerator.AsEnumerable<T>(this, reader))
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEntityFactory IDatabaseSet.Factory
        {
            get
            {
                return this.Factory;
            }
        }

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
