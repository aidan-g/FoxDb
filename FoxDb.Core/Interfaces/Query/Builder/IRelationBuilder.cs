namespace FoxDb.Interfaces
{
    public interface IRelationBuilder : IExpressionBuilder
    {
        IRelationConfig Relation { get; set; }
    }
}
