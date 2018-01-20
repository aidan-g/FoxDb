using FoxDb.Interfaces;

namespace FoxDb
{
    public class ParameterBuilder : ExpressionBuilder, IParameterBuilder
    {
        public ParameterBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Parameter;
            }
        }

        public string Name { get; set; }

        public override string DebugView
        {
            get
            {
                return string.Format("{{{0}}}", this.Name);
            }
        }
    }
}
