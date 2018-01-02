namespace FoxDb.Interfaces
{
    public interface ISubQueryBuilder : IExpressionBuilder
    {
        IQueryGraphBuilder Query { get; set; }
    }
}
