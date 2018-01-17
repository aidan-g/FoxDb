namespace FoxDb.Interfaces
{
    public interface ILimitBuilder : IFragmentBuilder
    {
        int Limit { get; }
    }
}
