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
            this.Table = table;
            this.Source = source;
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
                if (!this.Source.CanRead)
                {
                    throw new InvalidOperationException(string.Format("Query source cannot be read."));
                }
                var query = this.Database.QueryFactory.Create(this.Database.QueryFactory.Count(this.Source.Select));
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

        public T Find(object id)
        {
            if (!this.Source.CanSearch)
            {
                throw new InvalidOperationException(string.Format("Query source cannot be searched."));
            }
            var parameters = new PrimaryKeysParameterHandlerStrategy<T>(this.Database, id).Handler;
            var sequence = this.GetEnumerator(this.Source.Find, parameters);
            if (sequence.MoveNext())
            {
                return sequence.Current;
            }
            return default(T);
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (!this.Source.CanRead)
            {
                throw new InvalidOperationException(string.Format("Query source cannot be read."));
            }
            return this.GetEnumerator(this.Source.Select, this.Parameters);
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
    }
}
