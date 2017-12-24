using FoxDb.Interfaces;
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

        public IDatabaseQuery Create(string commandText, params string[] parameterNames)
        {
            return new DatabaseQuery(commandText, parameterNames);
        }

        public IDatabaseQueryComposer Compose()
        {
            return new SQLiteQueryComposer();
        }

        public IDatabaseQuery Select<T>() where T : IPersistable
        {
            var table = this.Database.Config.Table<T>();
            var composer = this.Compose()
                .Select()
                .Table(table)
                .IdentifierDelimiter()
                .Star()
                .From()
                .Table(table);
            return composer.Query;
        }

        public IDatabaseQuery Insert<T>() where T : IPersistable
        {
            var table = this.Database.Config.Table<T>();
            var composer = this.Compose()
                .Insert()
                .Table(table)
                .OpenParentheses()
                .Identifiers(table.Columns.Except(table.PrimaryKeys).Select(column => column.ColumnName))
                .CloseParentheses()
                .Select()
                .Parameters(table.Columns.Except(table.PrimaryKeys).Select(column => column.ColumnName))
                .Statement()
                .Select()
                .Identity();
            return composer.Query;
        }

        public IDatabaseQuery Insert<T1, T2>()
            where T1 : IPersistable
            where T2 : IPersistable
        {
            var table = this.Database.Config.Table<T1, T2>();
            var composer = this.Compose()
                .Insert()
                .Table(table)
                .OpenParentheses()
                .Identifiers(table.ForeignKeys.Select(column => column.ColumnName))
                .CloseParentheses()
                .Select()
                .Parameters(table.ForeignKeys.Select(column => column.ColumnName))
                .Statement()
                .Select()
                .Identity();
            return composer.Query;
        }

        public IDatabaseQuery Update<T>() where T : IPersistable
        {
            var table = this.Database.Config.Table<T>();
            var composer = this.Compose()
                .Update()
                .Table(table)
                .Set()
                .AssignParametersToIdentifiers(table.Columns.Except(table.PrimaryKeys).Select(column => column.ColumnName))
                .Where()
                .AssignParametersToColumns(table.PrimaryKeys);
            return composer.Query;
        }

        public IDatabaseQuery Delete<T>() where T : IPersistable
        {
            var table = this.Database.Config.Table<T>();
            var composer = this.Compose()
                .Delete()
                .From()
                .Table(table)
                .Where()
                .AssignParametersToColumns(table.PrimaryKeys);
            return composer.Query;
        }

        public IDatabaseQuery Delete<T1, T2>()
            where T1 : IPersistable
            where T2 : IPersistable
        {
            var table = this.Database.Config.Table<T1, T2>();
            var composer = this.Compose()
                .Delete()
                .From()
                .Table(table)
                .Where()
                .AssignParametersToColumns(table.ForeignKeys);
            return composer.Query;
        }

        public IDatabaseQuery Count(IDatabaseQuery query)
        {
            var composer = this.Compose()
                .Select()
                .Count()
                .From()
                .SubQuery(query);
            return composer.Query;
        }
    }
}
