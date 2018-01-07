namespace FoxDb.Interfaces
{
    public interface IQueryGraphVisitor
    {
        void Visit(IQueryGraph graph);

        void Visit(IFragmentBuilder fragment);
    }
}
