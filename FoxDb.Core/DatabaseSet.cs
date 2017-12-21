using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace FoxDb
{
    public class DatabaseSet<T> : IDatabaseSet<T> where T : IPersistable
    {
        public DatabaseSet(IDatabase database, IDatabaseQuery query, DatabaseParameterHandler parameters, IDbTransaction transaction)
        {
            this.Database = database;
            this.Query = query;
            this.Parameters = parameters;
            this.Transaction = transaction;
        }

        public IDatabase Database { get; private set; }

        public IDatabaseQuery Query { get; private set; }

        public DatabaseParameterHandler Parameters { get; private set; }

        public IDbTransaction Transaction { get; private set; }

        public int Count
        {
            get
            {
                var query = this.Database.QueryFactory.Count<T>(this.Query);
                return this.Database.Execute<int>(query, this.Parameters, this.Transaction);
            }
        }

        public void AddOrUpdate(T item)
        {
            this.AddOrUpdate(new[] { item });
        }

        public void AddOrUpdate(IEnumerable<T> items)
        {
            var add = this.Database.QueryFactory.Insert<T>();
            var update = this.Database.QueryFactory.Update<T>();
            foreach (var item in items)
            {
                if (item.HasId)
                {
                    this.Database.Execute(update, this.GetParameters(item), this.Transaction);
                }
                else
                {
                    item.Id = this.Database.Execute<object>(add, this.GetParameters(item), this.Transaction);
                }
            }
        }

        public void Clear()
        {
            this.Delete(this);
        }

        public void Delete(T item)
        {
            this.Delete(new[] { item });
        }

        public void Delete(IEnumerable<T> items)
        {
            var query = this.Database.QueryFactory.Delete<T>();
            foreach (var item in items)
            {
                this.Database.Execute(query, this.GetParameters(item), this.Transaction);
            }
        }

        public T Find(object id)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            var factory = new EntityFactory<T>();
            var populator = new EntityPopulator<T>();
            using (var reader = this.Database.ExecuteReader(this.Query, this.Parameters, this.Transaction))
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

        public DatabaseParameterHandler GetParameters(T item)
        {
            if (this.Parameters != null)
            {
                return this.Parameters;
            }
            return new SimpleParameterHandlerStrategy<T>(item).Handler;
        }
    }
}
