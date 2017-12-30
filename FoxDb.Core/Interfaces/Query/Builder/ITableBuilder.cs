namespace FoxDb.Interfaces
{
    public interface ITableBuilder : IExpressionBuilder
    {
        ITableConfig Table { get; set; }
    }
}
