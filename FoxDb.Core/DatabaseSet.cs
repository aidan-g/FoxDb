using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace FoxDb
{
    public class DatabaseSet<T> : IDatabaseSet<T>
    {
        public DatabaseSet(IDatabaseQuerySource source)
        {
            this.Source = source;
        }

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
                return this.Database.Config.Table<T>();
            }
        }

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
                var query = this.Database.QueryFactory.Create(this.Database.QueryFactory.Count(this.Source.Select));
                return this.Database.Execute<int>(query, this.Parameters, this.Transaction);
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
            var query = this.Database.SelectByPrimaryKey<T>();
            var parameters = new PrimaryKeysParameterHandlerStrategy<T>(this.Database, id).Handler;
            var sequence = this.GetEnumerator(query, parameters);
            if (sequence.MoveNext())
            {
                return sequence.Current;
            }
            return default(T);
        }

        public IEnumerator<T> GetEnumerator()
        {
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
