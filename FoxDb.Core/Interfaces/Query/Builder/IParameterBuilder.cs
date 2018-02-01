namespace FoxDb.Interfaces
{
    public interface IParameterBuilder : IExpressionBuilder
    {
        string Name { get; set; }

        ParameterType Type { get; set; }
    }
}
