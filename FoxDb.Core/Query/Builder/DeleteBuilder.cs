using FoxDb.Interfaces;

namespace FoxDb
{
    public class DeleteBuilder : FragmentBuilder, IDeleteBuilder
    {
        public DeleteBuilder(IQueryGraphBuilder graph) : base(graph)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Delete;
            }
        }
    }
}
