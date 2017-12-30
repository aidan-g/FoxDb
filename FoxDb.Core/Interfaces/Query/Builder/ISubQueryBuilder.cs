namespace FoxDb.Interfaces
{
    public interface ISubQueryBuilder : IExpressionBuilder
    {
        IDatabaseQuery Query { get; set; }
    }
}
