namespace FoxDb.Interfaces
{
    public interface IBinaryExpressionBuilder : IFragmentContainer, IFragmentTarget, IExpressionBuilder
    {
        bool IsLeaf { get; }

        IFragmentBuilder Left { get; set; }

        IOperatorBuilder Operator { get; set; }

        IFragmentBuilder Right { get; set; }

        T SetLeft<T>(T expression) where T : IFragmentBuilder;

        T SetRight<T>(T expression) where T : IFragmentBuilder;
    }
}
