namespace FoxDb.Interfaces
{
    public interface IFragmentTarget : IFragmentBuilder
    {
        string CommandText { get; }

        void Write(IFragmentBuilder fragment);
    }
}
