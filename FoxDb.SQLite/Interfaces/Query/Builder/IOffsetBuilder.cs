namespace FoxDb.Interfaces
{
    public interface IOffsetBuilder : IFragmentBuilder
    {
        int Offset { get; }
    }
}
