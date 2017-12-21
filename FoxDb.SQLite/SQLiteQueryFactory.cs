using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class SQLiteQueryFactory : IDatabaseQueryFactory
    {
        public SQLiteQueryFactory(IProvider provider)
        {
            this.Provider = provider;
        }

        public IProvider Provider { get; private set; }

        public IDatabaseQuery Count<T>(params string[] filters)
        {
            throw new NotImplementedException();
        }

        public IDatabaseQuery Delete<T>()
        {
            throw new NotImplementedException();
        }

        public IDatabaseQuery First<T>(params string[] filters)
        {
            throw new NotImplementedException();
        }

        public IDatabaseQuery Insert<T>()
        {
            throw new NotImplementedException();
        }

        public IDatabaseQuery Select<T>(params string[] filters)
        {
            throw new NotImplementedException();
        }

        public IDatabaseQuery Update<T>()
        {
            throw new NotImplementedException();
        }
    }
}
