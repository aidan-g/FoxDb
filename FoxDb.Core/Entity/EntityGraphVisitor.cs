using FoxDb.Interfaces;

namespace FoxDb
{
    public abstract class EntityGraphVisitor : IEntityGraphVisitor
    {
        public EntityGraphVisitor()
        {
            this.Members = new DynamicMethod(this.GetType());
        }

        public void Visit(IEntityGraph graph)
        {
            this.Visit(graph.Root);
        }

        protected DynamicMethod Members { get; private set; }

        public void Visit(IEntityGraphNode node)
        {
            if (node.Table != null)
            {
                this.Members.Invoke(this, "OnVisit", node.EntityType, node);
                if (node.Relation != null)
                {
                    this.Members.Invoke(this, "OnVisit", new[] { node.Parent.EntityType, node.Relation.RelationType }, node);
                }
            }
            foreach (var child in node.Children)
            {
                this.Visit(child);
            }
        }

        protected abstract void OnVisit<T>(IEntityGraphNode<T> node);

        protected abstract void OnVisit<T, TRelation>(IEntityGraphNode<T, TRelation> node);

        protected abstract void OnVisit<T, TRelation>(ICollectionEntityGraphNode<T, TRelation> node);
    }
}
