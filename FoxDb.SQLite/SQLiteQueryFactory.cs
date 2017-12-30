using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxDb
{
    public class SQLiteQueryFactory : IDatabaseQueryFactory
    {
        public SQLiteQueryFactory(IDatabase database)
        {
            this.Database = database;
        }

        public IDatabase Database { get; private set; }

        public IQueryGraphBuilder Build()
        {
            return new QueryGraphBuilder();
        }

        public IDatabaseQuery Create(params IQueryGraph[] graphs)
        {
            var builder = new StringBuilder();
            var parameterNames = new List<string>();
            foreach (var graph in graphs)
            {
                if (builder.Length > 0)
                {
                    builder.Append(SQLiteSyntax.STATEMENT);
                }
                var query = new SQLiteQueryBuilder(this.Database, graph).Query;
                builder.Append(query.CommandText);
                parameterNames.AddRange(query.ParameterNames.Except(parameterNames));
            }
            return new DatabaseQuery(builder.ToString(), parameterNames.ToArray());
        }

        public IDatabaseQuery Create(string commandText, params string[] parameterNames)
        {
            return new DatabaseQuery(commandText, parameterNames);
        }

        public IDatabaseQuery Select<T>()
        {
            var table = this.Database.Config.Table<T>();
            var builder = this.Build();
            builder.Select.AddColumns(table.Columns);
            builder.From.AddTable(table);
            builder.OrderBy.AddColumns(table.PrimaryKeys);
            return this.Create(builder.Build());
        }

        public IDatabaseQuery Insert<T>()
        {
            var graphs = new List<IQueryGraph>();
            var table = this.Database.Config.Table<T>();
            {
                var builder = this.Build();
                builder.Insert.SetTable(table);
                builder.Insert.AddColumns(table.Columns.Except(table.PrimaryKeys));
                builder.Select.AddParameters(table.Columns.Except(table.PrimaryKeys));
                graphs.Add(builder.Build());
            }
            {
                var builder = this.Build();
                builder.Select.AddFunction(QueryFunction.Identity);
                graphs.Add(builder.Build());
            }
            return this.Create(graphs.ToArray());
        }

        public IDatabaseQuery Insert<T1, T2>()
        {
            var table = this.Database.Config.Table<T1, T2>();
            var builder = this.Build();
            builder.Insert.SetTable(table);
            builder.Insert.AddColumns(table.ForeignKeys);
            builder.Select.AddParameters(table.ForeignKeys);
            return this.Create(builder.Build());
        }

        public IDatabaseQuery Update<T>()
        {
            var table = this.Database.Config.Table<T>();
            var builder = this.Build();
            builder.Update.SetTable(table);
            builder.Update.AddColumns(table.Columns.Except(table.PrimaryKeys));
            builder.Where.AddColumns(table.PrimaryKeys);
            return this.Create(builder.Build());
        }

        public IDatabaseQuery Delete<T>()
        {
            var table = this.Database.Config.Table<T>();
            var builder = this.Build();
            builder.Delete.Touch();
            builder.From.AddTable(table);
            builder.Where.AddColumns(table.PrimaryKeys);
            return this.Create(builder.Build());
        }

        public IDatabaseQuery Delete<T1, T2>()
        {
            var table = this.Database.Config.Table<T1, T2>();
            var builder = this.Build();
            builder.Delete.Touch();
            builder.From.AddTable(table);
            builder.Where.AddColumns(table.ForeignKeys);
            return this.Create(builder.Build());
        }

        public IDatabaseQuery Count(IDatabaseQuery query)
        {
            var builder = this.Build();
            builder.Select.AddFunction(QueryFunction.Count, builder.Select.GetOperator(QueryOperator.Star));
            builder.From.AddSubQuery(query);
            return this.Create(builder.Build());
        }
    }
}
