using FoxDb.Interfaces;
using FoxDb.Templates;
using System;

namespace FoxDb
{
    public class SQLiteQueryFactory : IDatabaseQueryFactory
    {
        public SQLiteQueryFactory(IDatabase database)
        {
            this.Database = database;
        }

        public IDatabase Database { get; private set; }

        public IDatabaseQuery Count<T>(params string[] filters)
        {
            throw new NotImplementedException();
        }

        public IDatabaseQuery Delete<T>()
        {
            var table = this.Database.Config.Table<T>();
            var delete = new Delete(table.Name, table.Key.Name);
            return new DatabaseQuery(delete.TransformText(), table.Key.Name);
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
            var table = this.Database.Config.Table<T>();
            var select = new Select(table.Name, filters);
            return new DatabaseQuery(select.TransformText(), filters);
        }

        public IDatabaseQuery Update<T>()
        {
            throw new NotImplementedException();
        }
    }
}
