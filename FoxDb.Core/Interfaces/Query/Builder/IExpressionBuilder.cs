namespace FoxDb.Interfaces
{
    public interface IExpressionBuilder : IFragmentBuilder
    {
        string Alias { get; set; }
    }
}
