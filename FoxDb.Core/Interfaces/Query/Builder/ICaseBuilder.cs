namespace FoxDb.Interfaces
{
    public interface ICaseBuilder : IFragmentContainer, IFragmentTarget, IExpressionBuilder
    {
        ICaseConditionBuilder Add();

        ICaseConditionBuilder Add(IFragmentBuilder result);

        ICaseConditionBuilder Add(IFragmentBuilder condition, IFragmentBuilder result);
    }
}
