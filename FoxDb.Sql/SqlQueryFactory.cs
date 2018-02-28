﻿using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxDb
{
    public abstract class SqlQueryFactory : IDatabaseQueryFactory
    {
        public SqlQueryFactory(IDatabase database)
        {
            this.Database = database;
        }

        public IDatabase Database { get; private set; }

        public abstract IDatabaseQueryDialect Dialect { get; }

        public IQueryGraphBuilder Build()
        {
            return new QueryGraphBuilder(this.Database);
        }

        public IDatabaseQuery Create(IQueryGraphBuilder graph)
        {
            if (graph is AggregateQueryGraphBuilder)
            {
                return this.Create((graph as AggregateQueryGraphBuilder).Queries);
            }
            return this.CreateBuilder(this.Database, graph).Query;
        }

        protected virtual IDatabaseQuery Create(IEnumerable<IQueryGraphBuilder> graphs)
        {
            var builder = new StringBuilder();
            var parameters = new List<IDatabaseQueryParameter>();
            foreach (var graph in graphs)
            {
                if (builder.Length > 0)
                {
                    builder.Append(this.Dialect.BATCH);
                }
                var query = this.Create(graph);
                builder.Append(query.CommandText);
                parameters.AddRange(query.Parameters.Except(parameters));
            }
            return this.Create(builder.ToString(), parameters.ToArray());
        }

        public abstract IDatabaseQuery Create(string commandText, params IDatabaseQueryParameter[] parameters);

        public IQueryGraphBuilder Combine(IEnumerable<IQueryGraphBuilder> graphs)
        {
            return new AggregateQueryGraphBuilder(this.Database, graphs);
        }

        public IDatabaseQuery Combine(IEnumerable<IDatabaseQuery> queries)
        {
            var builder = new StringBuilder();
            var parameters = new List<IDatabaseQueryParameter>();
            foreach (var query in queries)
            {
                if (builder.Length > 0)
                {
                    builder.AppendFormat("{0} ", this.Dialect.BATCH);
                }
                builder.Append(query.CommandText);
                parameters.AddRange(query.Parameters);
            }
            return this.Create(builder.ToString(), parameters.ToArray());
        }

        public virtual IQueryGraphBuilder Fetch(ITableConfig table)
        {
            var builder = this.Build();
            builder.Output.AddColumns(table.Columns);
            builder.Source.AddTable(table);
            builder.Sort.AddColumns(table.PrimaryKeys);
            return builder;
        }

        public virtual IQueryGraphBuilder Add(ITableConfig table)
        {
            var builder = this.Build();
            var columns = table.UpdatableColumns;
            builder.Add.SetTable(table);
            if (columns.Any())
            {
                builder.Add.AddColumns(columns);
                builder.Output.AddParameters(columns);
            }
            return builder;
        }

        public virtual IQueryGraphBuilder Update(ITableConfig table)
        {
            var builder = this.Build();
            builder.Update.SetTable(table);
            builder.Update.AddColumns(table.UpdatableColumns);
            builder.Filter.AddColumns(table.PrimaryKeys);
            return builder;
        }

        public virtual IQueryGraphBuilder Delete(ITableConfig table)
        {
            return this.Delete(table, table.PrimaryKeys);
        }

        public virtual IQueryGraphBuilder Delete(ITableConfig table, IEnumerable<IColumnConfig> keys)
        {
            var builder = this.Build();
            builder.Delete.Touch();
            builder.Source.AddTable(table);
            builder.Filter.AddColumns(keys);
            return builder;
        }

        public virtual IQueryGraphBuilder Count(IQueryGraphBuilder query)
        {
            var builder = this.Build();
            builder.Output.AddFunction(QueryFunction.Count, builder.Output.CreateOperator(QueryOperator.Star));
            builder.Source.AddSubQuery(query);
            return builder;
        }

        public abstract IQueryGraphBuilder Count(ITableConfig table, IQueryGraphBuilder query);

        protected abstract IQueryBuilder CreateBuilder(IDatabase database, IQueryGraphBuilder graph);
    }
}