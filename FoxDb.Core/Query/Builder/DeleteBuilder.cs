using FoxDb.Interfaces;

namespace FoxDb
{
    public class DeleteBuilder : FragmentBuilder, IDeleteBuilder
    {
        public DeleteBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Delete;
            }
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IDeleteBuilder>();
        }
    }
}
