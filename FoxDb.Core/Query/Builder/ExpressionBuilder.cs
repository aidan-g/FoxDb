using FoxDb.Interfaces;

namespace FoxDb
{
    public abstract class ExpressionBuilder : FragmentBuilder, IExpressionBuilder
    {
        protected ExpressionBuilder(IQueryGraphBuilder graph) : base(graph)
        {

        }

        public string Alias { get; set; }
    }
}
