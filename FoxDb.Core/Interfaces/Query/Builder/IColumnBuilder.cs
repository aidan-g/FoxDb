namespace FoxDb.Interfaces
{
    public interface IColumnBuilder : IExpressionBuilder
    {
        IColumnConfig Column { get; set; }
    }
}
