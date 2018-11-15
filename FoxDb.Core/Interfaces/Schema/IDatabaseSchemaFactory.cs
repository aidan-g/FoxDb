using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSchemaFactory
    {
        IDatabaseQueryDialect Dialect { get; }

        ISchemaGraphBuilder Build();

        ISchemaGraphBuilder Combine(IEnumerable<ISchemaGraphBuilder> graphs);

        ISchemaGraphBuilder Add(ITableConfig table, SchemaFlags flags);

        ISchemaGraphBuilder Update(ITableConfig leftTable, ITableConfig rightTable, SchemaFlags flags);

        ISchemaGraphBuilder Delete(ITableConfig table, SchemaFlags flags);
    }
}
