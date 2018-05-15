namespace FoxDb.Interfaces
{
    public interface IDatabaseSchemaFactory
    {
        IDatabaseQueryDialect Dialect { get; }

        ISchemaGraphBuilder Build();

        ISchemaGraphBuilder Add(ITableConfig table);

        ISchemaGraphBuilder Update(ITableConfig leftTable, ITableConfig rightTable);

        ISchemaGraphBuilder Delete(ITableConfig table);
    }
}
