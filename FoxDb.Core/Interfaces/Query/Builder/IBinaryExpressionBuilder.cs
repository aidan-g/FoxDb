namespace FoxDb.Interfaces
{
    public interface IBinaryExpressionBuilder : IExpressionBuilder, IFragmentTarget
    {
        IExpressionBuilder Left { get; set; }

        IOperatorBuilder Operator { get; set; }

        IExpressionBuilder Right { get; set; }
    }
}
