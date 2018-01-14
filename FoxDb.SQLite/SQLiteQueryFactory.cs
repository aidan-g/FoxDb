using FoxDb.Interfaces;
using System;
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

        public IDatabaseQuery Create(IEnumerable<IQueryGraph> graphs)
        {
            return this.Create(graphs.ToArray());
        }

        public IDatabaseQuery Create(params IQueryGraphBuilder[] builders)
        {
            var graphs = builders.Select(builder => builder.Build());
            return this.Create(graphs.ToArray());
        }

        public IDatabaseQuery Create(IEnumerable<IQueryGraphBuilder> builders)
        {
            return this.Create(builders.ToArray());
        }

        public IDatabaseQuery Create(string commandText, params string[] parameterNames)
        {
            return new DatabaseQuery(commandText, parameterNames);
        }

        public IQueryGraphBuilder Select(ITableConfig table)
        {
            var builder = this.Build();
            builder.Select.AddColumns(table.Columns);
            builder.From.AddTable(table);
            builder.OrderBy.AddColumns(table.PrimaryKeys);
            return builder;
        }

        public IEnumerable<IQueryGraphBuilder> Insert(ITableConfig table)
        {
            var graphs = new List<IQueryGraphBuilder>();
            {
                var builder = this.Build();
                var columns = table.Columns.Except(table.PrimaryKeys);
                builder.Insert.SetTable(table);
                if (columns.Any())
                {
                    builder.Insert.AddColumns(columns);
                    builder.Select.AddParameters(columns);
                }
                graphs.Add(builder);
            }
            {
                var builder = this.Build();
                builder.Select.AddFunction(QueryFunction.Identity);
                graphs.Add(builder);
            }
            return graphs;
        }

        public IQueryGraphBuilder Update(ITableConfig table)
        {
            var builder = this.Build();
            builder.Update.SetTable(table);
            builder.Update.AddColumns(table.Columns.Except(table.PrimaryKeys));
            builder.Where.AddColumns(table.PrimaryKeys);
            return builder;
        }

        public IQueryGraphBuilder Delete(ITableConfig table)
        {
            return this.Delete(table, table.PrimaryKeys);
        }

        public IQueryGraphBuilder Delete(ITableConfig table, IEnumerable<IColumnConfig> keys)
        {
            var builder = this.Build();
            builder.Delete.Touch();
            builder.From.AddTable(table);
            builder.Where.AddColumns(keys);
            return builder;
        }

        public IQueryGraphBuilder Count(IQueryGraphBuilder query)
        {
            var builder = this.Build();
            builder.Select.AddFunction(QueryFunction.Count, builder.Select.GetOperator(QueryOperator.Star));
            builder.From.AddSubQuery(query);
            return builder;
        }
    }
}
