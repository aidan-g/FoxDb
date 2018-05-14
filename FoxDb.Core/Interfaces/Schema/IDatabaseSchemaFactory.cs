namespace FoxDb.Interfaces
{
    public interface IDatabaseSchemaFactory
    {
        IDatabaseQueryDialect Dialect { get; }

        ISchemaGraphBuilder Build();

        ISchemaGraphBuilder Create(ITableConfig table);

        ISchemaGraphBuilder Alter(ITableConfig leftTable, ITableConfig rightTable);

        ISchemaGraphBuilder Drop(ITableConfig table);
    }
}
