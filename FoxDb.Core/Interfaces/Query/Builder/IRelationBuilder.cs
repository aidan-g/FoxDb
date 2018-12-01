namespace FoxDb.Interfaces
{
    public interface IRelationBuilder : IExpressionBuilder
    {
        ITableConfig LeftTable { get; }

        ITableConfig RightTable { get; }

        IRelationConfig Relation { get; set; }
    }
}