using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class EntityGraphBuilder : IEntityGraphBuilder
    {
        protected EntityGraphBuilder()
        {
            this.Members = new DynamicMethod<EntityGraphBuilder>();
        }

        public EntityGraphBuilder(params IEntityGraphMapping[] mapping)
            : this()
        {
            this.Mapping = mapping.ToDictionary(map => map.Table);
        }

        protected DynamicMethod<EntityGraphBuilder> Members { get; private set; }

        public IDictionary<ITableConfig, IEntityGraphMapping> Mapping { get; private set; }

        protected virtual IEntityGraphMapping GetMapping(ITableConfig table)
        {
            var mapping = default(IEntityGraphMapping);
            if (!this.Mapping.TryGetValue(table, out mapping))
            {
                mapping = this.Mapping[table] = new EntityGraphMapping(table);
            }
            return mapping;
        }

        public IEntityGraph Build(ITableConfig table, IEntityMapper mapper)
        {
            return new EntityGraph(this.CreateNode(table, mapper));
        }

        protected virtual IEntityGraphNode CreateNode(ITableConfig table, IEntityMapper mapper)
        {
            var node = (EntityGraphNode)this.Members.Invoke(this, "CreateNode", this.GetMapping(table).EntityType, table);
            node.Children = GetChildren(this, node, mapper);
            return node;
        }

        protected virtual IEntityGraphNode<T> CreateNode<T>(ITableConfig table)
        {
            return new EntityGraphNode<T>(table);
        }

        protected virtual IEntityGraphNode CreateNode(IEntityGraphNode parent, IRelationConfig relation, IEntityMapper mapper)
        {
            var node = (EntityGraphNode)this.Members.Invoke(this, "CreateNode", new[] { this.GetMapping(parent.Table).EntityType, relation.RelationType }, parent, relation);
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
            var children = new List<IEntityGraphNode>();
            foreach (var relation in parent.Table.Relations)
            {
                if (!mapper.Relations.Contains(relation))
                {
                    continue;
                }
                if (children.Any(child => TableComparer.TableConfig.Equals(child.Relation.RightTable, relation.RightTable)))
                {
                    continue;
                }
                children.Add(builder.CreateNode(parent, relation, mapper));
            }
            return children;
        }
    }
}
