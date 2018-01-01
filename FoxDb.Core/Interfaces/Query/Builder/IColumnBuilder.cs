namespace FoxDb.Interfaces
{
    public interface IColumnBuilder : IExpressionBuilder
    {
        OrderByDirection Direction { get; set; }

        IColumnConfig Column { get; set; }
    }

    public enum OrderByDirection : byte
    {
        None,
        Ascending,
        Descending
    }
}
