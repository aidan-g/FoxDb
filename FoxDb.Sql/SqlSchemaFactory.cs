using FoxDb.Interfaces;
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
            var builder = this.Build();
            builder.Create.SetTable(table);
            builder.Create.AddColumns(table.Columns);
            builder.Create.AddIndexes(table.Indexes);
            if (table.Relations.Any())
            {
                var calculator = new EntityRelationCalculator(table);
                calculator.AddRelations(table.Relations);
                return new AggregateSchemaGraphBuilder(
                    this.Database,
                    calculator.CalculatedRelations.Select(
                        relation => this.Add(relation.Table)
                    ).Concat(builder)
                );
            }
            return builder;
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
            var builder = this.Build();
            builder.Drop.SetTable(table);
            if (table.Relations.Any())
            {
                var calculator = new EntityRelationCalculator(table);
                calculator.AddRelations(table.Relations);
                return new AggregateSchemaGraphBuilder(
                    this.Database,
                    calculator.CalculatedRelations.Select(
                        relation => this.Delete(relation.Table)
                    ).Concat(builder)
                );
            }
            return builder;
        }
    }
}
