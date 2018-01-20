using FoxDb.Interfaces;

namespace FoxDb
{
    public abstract class ExpressionBuilder : FragmentBuilder, IExpressionBuilder
    {
        protected ExpressionBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {

        }

        public string Alias { get; set; }
    }
}
