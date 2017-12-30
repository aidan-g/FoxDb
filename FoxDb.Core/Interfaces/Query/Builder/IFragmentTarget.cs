namespace FoxDb.Interfaces
{
    public interface IFragmentTarget : IFragmentBuilder
    {
        void Write(IFragmentBuilder fragment);
    }
}
