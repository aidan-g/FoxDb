namespace FoxDb.Interfaces
{
    public interface IValueGeneratorStrategy
    {
        object CreateValue(ITableConfig table, IColumnConfig column, object item);
    }
}
