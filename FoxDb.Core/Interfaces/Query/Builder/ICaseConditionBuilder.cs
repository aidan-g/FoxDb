namespace FoxDb.Interfaces
{
    public interface ICaseConditionBuilder : IFragmentContainer, IFragmentTarget, IExpressionBuilder
    {
        IFragmentBuilder Condition { get; set; }

        IFragmentBuilder Result { get; set; }
    }
}
