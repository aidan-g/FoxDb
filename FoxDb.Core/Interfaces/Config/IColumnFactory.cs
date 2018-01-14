namespace FoxDb.Interfaces
{
    public interface IColumnFactory
    {
        IColumnConfig Create(ITableConfig table, IColumnSelector selector);
    }
}
