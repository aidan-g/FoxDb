using FoxDb.Interfaces;

namespace FoxDb
{
    public class ConstantBuilder : ExpressionBuilder, IConstantBuilder
    {
        public ConstantBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Constant;
            }
        }

        public object Value { get; set; }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IConstantBuilder>().With(builder => builder.Value = this.Value);
        }

        public override string DebugView
        {
            get
            {
                return string.Format("{{{0}}}", this.Value);
            }
        }
    }
}
