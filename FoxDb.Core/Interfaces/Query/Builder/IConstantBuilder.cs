namespace FoxDb.Interfaces
{
    public interface IConstantBuilder : IExpressionBuilder
    {
        object Value { get; set; }
    }
}
