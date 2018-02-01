using FoxDb.Interfaces;

namespace FoxDb
{
    public class ParameterBuilder : ExpressionBuilder, IParameterBuilder
    {
        public ParameterBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {
            this.Type = ParameterType.Input;
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Parameter;
            }
        }

        public string Name { get; set; }

        public ParameterType Type { get; set; }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IParameterBuilder>().With(builder => builder.Name = this.Name);
        }

        public override string DebugView
        {
            get
            {
                return string.Format("{{{0}}}", this.Name);
            }
        }
    }
}
