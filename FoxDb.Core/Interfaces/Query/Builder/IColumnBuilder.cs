namespace FoxDb.Interfaces
{
    public interface IColumnBuilder : IFragmentContainer, IExpressionBuilder
    {
        IColumnConfig Column { get; set; }

        OrderByDirection Direction { get; set; }

        ColumnBuilderFlags Flags { get; set; }
    }

    public enum OrderByDirection : byte
    {
        None,
        Ascending,
        Descending
    }

    public enum ColumnBuilderFlags : byte
    {
        None,
        Identifier,
        Distinct
    }
}
