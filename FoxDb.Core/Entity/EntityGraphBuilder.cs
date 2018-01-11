using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class EntityGraphBuilder : IEntityGraphBuilder
    {
        public EntityGraphBuilder()
        {
            this.Members = new DynamicMethod(this.GetType());
        }

        protected DynamicMethod Members { get; private set; }

        public IEntityGraph Build<T>(ITableConfig table, IEntityMapper mapper)
        {
            return new EntityGraph(this.CreateNode(table, mapper));
        }

        protected virtual IEntityGraphNode CreateNode(ITableConfig table, IEntityMapper mapper)
        {
            var node = (EntityGraphNode)this.Members.Invoke(this, "CreateNode", new[] { table.TableType }, table);
            node.Children = GetChildren(this, node, mapper);
            return node;
        }

        protected virtual IEntityGraphNode<T> CreateNode<T>(ITableConfig table)
        {
            return new EntityGraphNode<T>(table);
        }

        protected virtual IEntityGraphNode CreateNode(IEntityGraphNode parent, IRelationConfig relation, IEntityMapper mapper)
        {
            var node = (EntityGraphNode)this.Members.Invoke(this, "CreateNode", new[] { parent.Table.TableType, relation.RelationType }, parent, relation);
            node.Children = GetChildren(this, node, mapper);
            return node;
        }

        protected virtual IEntityGraphNode<TRelation> CreateNode<T, TRelation>(IEntityGraphNode<T> parent, IRelationConfig<T, TRelation> relation)
            where T : class
            where TRelation : class
        {
            return new EntityRelationGraphNode<T, TRelation>(parent, relation);
        }

        protected virtual EntityGraphNode<TRelation> CreateNode<T, TRelation>(IEntityGraphNode<T> parent, ICollectionRelationConfig<T, TRelation> relation)
            where T : class
            where TRelation : class
        {
            return new CollectionEntityRelationGraphNode<T, TRelation>(parent, relation);
        }

        protected static IEnumerable<IEntityGraphNode> GetChildren(EntityGraphBuilder builder, IEntityGraphNode parent, IEntityMapper mapper)
        {
            foreach (var relation in parent.Table.Relations)
            {
                if (!mapper.Relations.Contains(relation))
                {
                    continue;
                }
                yield return builder.CreateNode(parent, relation, mapper);
            }
        }
    }
}
