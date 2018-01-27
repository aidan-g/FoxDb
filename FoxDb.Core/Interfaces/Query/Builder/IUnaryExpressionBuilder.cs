namespace FoxDb.Interfaces
{
    public interface IUnaryExpressionBuilder : IFragmentContainer, IFragmentTarget, IExpressionBuilder
    {
        bool IsLeaf { get; }

        IOperatorBuilder Operator { get; set; }

        IFragmentBuilder Expression { get; set; }
    }
}