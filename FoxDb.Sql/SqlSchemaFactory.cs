using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public abstract class SqlSchemaFactory : IDatabaseSchemaFactory
    {
        public SqlSchemaFactory(IDatabase database)
        {
            this.Database = database;
        }

        public IDatabase Database { get; private set; }

        public abstract IDatabaseQueryDialect Dialect { get; }

        public ISchemaGraphBuilder Build()
        {
            return new SchemaGraphBuilder(this.Database);
        }

        public ISchemaGraphBuilder Combine(IEnumerable<ISchemaGraphBuilder> graphs)
        {
            return new AggregateSchemaGraphBuilder(this.Database, graphs);
        }

        public ISchemaGraphBuilder Add(ITableConfig table)
        {
            var tableFactory = new Func<ITableConfig, ISchemaGraphBuilder>(element =>
            {
                var builder = this.Build();
                builder.Create.AddTable(element);
                builder.Create.AddColumns(element.Columns);
                builder.Create.AddIndexes(element.Indexes);
                return builder;
            });
            var relationFactory = new Func<ITableConfig, ISchemaGraphBuilder>(element =>
            {
                var builder = this.Build();
                builder.Create.AddRelations(element.Relations);
                return builder;
            });
            if (table.Relations.Any())
            {
                return new AggregateSchemaGraphBuilder(
                    this.Database,
                    new[] { tableFactory(table), relationFactory(table) }
                );
            }
            return tableFactory(table);
        }

        public ISchemaGraphBuilder Update(ITableConfig leftTable, ITableConfig rightTable)
        {
            var builder = this.Build();
            builder.Alter.SetLeftTable(leftTable);
            builder.Alter.SetRightTable(rightTable);
            return builder;
        }

        public ISchemaGraphBuilder Delete(ITableConfig table)
        {
            var tableFactory = new Func<ITableConfig, ISchemaGraphBuilder>(element =>
            {
                var builder = this.Build();
                builder.Drop.AddTable(element);
                builder.Drop.AddIndexes(element.Indexes);
                return builder;
            });
            var relationFactory = new Func<ITableConfig, ISchemaGraphBuilder>(element =>
            {
                var builder = this.Build();
                builder.Drop.AddRelations(element.Relations);
                return builder;
            });
            if (table.Relations.Any())
            {
                return new AggregateSchemaGraphBuilder(
                    this.Database,
                    new[] { relationFactory(table), tableFactory(table) }
                );
            }
            return tableFactory(table);
        }
    }
}
