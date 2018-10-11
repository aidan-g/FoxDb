using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSchemaFactory
    {
        IDatabaseQueryDialect Dialect { get; }

        ISchemaGraphBuilder Build();

        ISchemaGraphBuilder Combine(IEnumerable<ISchemaGraphBuilder> graphs);

        ISchemaGraphBuilder Add(ITableConfig table);

        ISchemaGraphBuilder Update(ITableConfig leftTable, ITableConfig rightTable);

        ISchemaGraphBuilder Delete(ITableConfig table);
    }
}
