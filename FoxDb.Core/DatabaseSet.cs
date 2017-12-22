﻿using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace FoxDb
{
    public class DatabaseSet<T> : IDatabaseSet<T> where T : IPersistable
    {
        public DatabaseSet(IDatabaseQuerySource<T> source)
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

        public IDatabaseQuerySource<T> Source { get; private set; }

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
                var query = this.Database.QueryFactory.Count<T>(this.Source.Select);
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
            var table = this.Database.Config.Table<T>();
            var query = this.Database.QueryFactory.Find<T>();
            var parameters = new KeyParameterHandlerStrategy<T>(this.Database, id).Handler;
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

        protected virtual IEnumerator<T> GetEnumerator(IDatabaseQuery query, DatabaseParameterHandler parameters)
        {
            var factory = new EntityFactory<T>(this);
            var populator = new EntityPopulator<T>(this);
            using (var reader = this.Database.ExecuteReader(query, parameters, this.Transaction))
            {
                foreach (var record in reader)
                {
                    var item = factory.Create();
                    populator.Populate(item, record);
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
