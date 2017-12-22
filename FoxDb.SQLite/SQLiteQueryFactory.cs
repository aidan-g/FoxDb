using FoxDb.Interfaces;
using FoxDb.Templates;
using System;
using System.Linq;

namespace FoxDb
{
    public class SQLiteQueryFactory : IDatabaseQueryFactory
    {
        public SQLiteQueryFactory(IDatabase database)
        {
            this.Database = database;
        }

        public IDatabase Database { get; private set; }

        public IDatabaseQueryCriteria Criteria<T>(string column) where T : IPersistable
        {
            var table = this.Database.Config.Table<T>();
            return new DatabaseQueryCriteria(table.Name, column);
        }

        public IDatabaseQueryCriteria Criteria<T1, T2>(string column) where T1 : IPersistable where T2 : IPersistable
        {
            var table = this.Database.Config.Table<T1, T2>();
            return new DatabaseQueryCriteria(table.Name, column);
        }

        public IDatabaseQuery Create(string commandText, params string[] parameterNames)
        {
            return new DatabaseQuery(commandText, parameterNames);
        }

        public IDatabaseQuery Count<T>(params IDatabaseQueryCriteria[] criteria) where T : IPersistable
        {
            throw new NotImplementedException();
        }

        public IDatabaseQuery Count<T>(IDatabaseQuery query) where T : IPersistable
        {
            var count = new Count(query.CommandText);
            return new DatabaseQuery(count.TransformText(), query.ParameterNames);
        }

        public IDatabaseQuery Delete<T>() where T : IPersistable
        {
            var table = this.Database.Config.Table<T>();
            var delete = new Delete(table.Name, table.Key.Name);
            return new DatabaseQuery(delete.TransformText(), table.Key.Name);
        }

        public IDatabaseQuery First<T>(params IDatabaseQueryCriteria[] criteria) where T : IPersistable
        {
            throw new NotImplementedException();
        }

        public IDatabaseQuery Insert<T>() where T : IPersistable
        {
            var table = this.Database.Config.Table<T>();
            var fields = SQLiteSchema.GetFieldNames(this.Database, table.Name);
            var insert = new Insert(table.Name, fields);
            return new DatabaseQuery(insert.TransformText(), fields);
        }

        public IDatabaseQuery Insert<T1, T2>() where T1 : IPersistable where T2 : IPersistable
        {
            var table = this.Database.Config.Table<T1, T2>();
            var fields = SQLiteSchema.GetFieldNames(this.Database, table.Name);
            var insert = new Insert(table.Name, fields);
            return new DatabaseQuery(insert.TransformText(), fields);
        }

        public IDatabaseQuery Select<T>(params IDatabaseQueryCriteria[] criteria) where T : IPersistable
        {
            var table = this.Database.Config.Table<T>();
            var select = new Select(table.Name, criteria);
            return new DatabaseQuery(select.TransformText(), criteria.ToParameters());
        }

        public IDatabaseQuery Select<T1, T2>(params IDatabaseQueryCriteria[] criteria) where T1 : IPersistable where T2 : IPersistable
        {
            var table1 = this.Database.Config.Table<T2>();
            var table2 = this.Database.Config.Table<T1, T2>();
            var column1 = Conventions.KeyColumn;
            var column2 = Conventions.RelationColumn(typeof(T2));
            var join = new Join(table1.Name, table2.Name, column1, column2, criteria);
            return new DatabaseQuery(join.TransformText(), criteria.ToParameters());
        }

        public IDatabaseQuery Update<T>() where T : IPersistable
        {
            var table = this.Database.Config.Table<T>();
            var fields = SQLiteSchema.GetFieldNames(this.Database, table.Name);
            var update = new Update(table.Name, table.Key.Name, fields);
            return new DatabaseQuery(update.TransformText(), fields);
        }
    }
}
