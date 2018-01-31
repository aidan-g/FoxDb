namespace FoxDb.Interfaces
{
    public interface IQueryGraphVisitor
    {
        void Visit(IQueryGraphBuilder graph);

        void Visit(IFragmentBuilder parent, IQueryGraphBuilder graph, IFragmentBuilder fragment);

        IDatabaseQuery Query { get; }
    }
}
