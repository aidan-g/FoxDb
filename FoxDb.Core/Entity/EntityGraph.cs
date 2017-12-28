using FoxDb.Interfaces;

namespace FoxDb
{
    public class EntityGraph : IEntityGraph
    {
        public EntityGraph(IEntityGraphNode root)
        {
            this.Root = root;
        }

        public IEntityGraphNode Root { get; private set; }
    }
}
