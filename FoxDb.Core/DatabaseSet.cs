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
        }

        public IDatabase Database { get; private set; }

        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void AddOrUpdate(T item)
        {
            this.AddOrUpdate(new[] { item });
        }

        public void AddOrUpdate(IEnumerable<T> items)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public T Find(object id)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
