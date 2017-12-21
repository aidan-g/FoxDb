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

        public IDatabaseQuery Create(string commandText, params string[] parameterNames)
        {
            return new DatabaseQuery(commandText, parameterNames);
        }

        public IDatabaseQuery Count<T>(params string[] filters)
        {
            throw new NotImplementedException();
        }

        public IDatabaseQuery Count<T>(IDatabaseQuery query)
        {
            var count = new Count(query.CommandText);
            return new DatabaseQuery(count.TransformText(), query.ParameterNames);
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
            var table = this.Database.Config.Table<T>();
            var fields = SQLiteSchema.GetFieldNames(this.Database, table.Name);
            var insert = new Insert(table.Name, fields);
            return new DatabaseQuery(insert.TransformText(), fields);
        }

        public IDatabaseQuery Select<T>(params string[] filters)
        {
            var table = this.Database.Config.Table<T>();
            var select = new Select(table.Name, filters);
            return new DatabaseQuery(select.TransformText(), filters);
        }

        public IDatabaseQuery Update<T>()
        {
            var table = this.Database.Config.Table<T>();
            var fields = SQLiteSchema.GetFieldNames(this.Database, table.Name);
            var update = new Update(table.Name, table.Key.Name, fields);
            return new DatabaseQuery(update.TransformText(), fields);
        }
    }
}
