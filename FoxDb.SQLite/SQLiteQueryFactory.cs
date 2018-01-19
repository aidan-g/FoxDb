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

        public IQueryGraphBuilder Build(params IQueryGraphBuilder[] queries)
        {
            if (queries.Any())
            {
                return new AggregateQueryGraphBuilder(queries);
            }
            else
            {
                return new QueryGraphBuilder();
            }
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
            var graphs = builders.SelectMany(builder => builder.Build());
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

        public IQueryGraphBuilder Fetch(ITableConfig table)
        {
            var builder = this.Build();
            builder.Output.AddColumns(table.Columns);
            builder.Source.AddTable(table);
            builder.Sort.AddColumns(table.PrimaryKeys);
            return builder;
        }

        public IQueryGraphBuilder Add(ITableConfig table)
        {
            var queries = new List<IQueryGraphBuilder>();
            {
                var builder = this.Build();
                var columns = table.Columns.Except(table.PrimaryKeys);
                builder.Add.SetTable(table);
                if (columns.Any())
                {
                    builder.Add.AddColumns(columns);
                    builder.Output.AddParameters(columns);
                }
                queries.Add(builder);
            }
            {
                var builder = this.Build();
                builder.Output.AddFunction(QueryFunction.Identity);
                queries.Add(builder);
            }
            return this.Build(queries.ToArray());
        }

        public IQueryGraphBuilder Update(ITableConfig table)
        {
            var builder = this.Build();
            builder.Update.SetTable(table);
            builder.Update.AddColumns(table.Columns.Except(table.PrimaryKeys));
            builder.Filter.AddColumns(table.PrimaryKeys);
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
            builder.Source.AddTable(table);
            builder.Filter.AddColumns(keys);
            return builder;
        }

        public IQueryGraphBuilder Count(IQueryGraphBuilder query)
        {
            var builder = this.Build();
            builder.Output.AddFunction(QueryFunction.Count, builder.Output.CreateOperator(QueryOperator.Star));
            builder.Source.AddSubQuery(query);
            return builder;
        }

        public IQueryGraphBuilder Count(ITableConfig table, IQueryGraphBuilder query)
        {
            var builder = this.Build();
            builder.Output.AddFunction(
                QueryFunction.Count,
                builder.Output.CreateColumn(table.PrimaryKey).With(
                    column => column.Flags = ColumnBuilderFlags.Identifier | ColumnBuilderFlags.Distinct
                )
            );
            builder.Source.AddSubQuery(query).With(
                subQuery => subQuery.Alias = table.TableName
            );
            return builder;
        }
    }
}
