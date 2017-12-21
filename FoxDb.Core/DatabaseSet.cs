using FoxDb.Interfaces;
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

        public void AddOrUpdate(T item)
        {
            this.AddOrUpdate(new[] { item });
        }

        public void AddOrUpdate(IEnumerable<T> items)
        {
            var persister = new EntityPersister<T>(this);
            foreach (var item in items)
            {
                persister.AddOrUpdate(item);
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
            var persister = new EntityPersister<T>(this);
            foreach (var item in items)
            {
                persister.Delete(item);
            }
        }

        public T Find(object id)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            var factory = new EntityFactory<T>(this);
            var populator = new EntityPopulator<T>(this);
            using (var reader = this.Database.ExecuteReader(this.Source.Select, this.Parameters, this.Transaction))
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
