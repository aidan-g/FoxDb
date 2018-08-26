namespace FoxDb.Interfaces
{
    public interface IIdentifierBuilder : IExpressionBuilder
    {
        string Identifier { get; set; }
    }
}
