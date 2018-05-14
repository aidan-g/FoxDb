using FoxDb.Interfaces;
using System;

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

        public ISchemaGraphBuilder Create(ITableConfig table)
        {
            var builder = this.Build();
            builder.Create.SetTable(table);
            builder.Create.AddColumns(table.Columns);
            return builder;
        }

        public ISchemaGraphBuilder Alter(ITableConfig leftTable, ITableConfig rightTable)
        {
            throw new NotImplementedException();
        }

        public ISchemaGraphBuilder Drop(ITableConfig table)
        {
            var builder = this.Build();
            builder.Drop.SetTable(table);
            return builder;
        }
    }
}
