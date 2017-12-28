using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public abstract class EntityGraphNode : IEntityGraphNode
    {
        public EntityGraphNode(ITableConfig table, IRelationConfig relation)
        {
            this.Table = table;
            this.Relation = relation;
        }

        public ITableConfig Table { get; private set; }

        public IRelationConfig Relation { get; private set; }

        public IEnumerable<IEntityGraphNode> Children { get; set; }
    }

    public class EntityGraphNode<T> : EntityGraphNode, IEntityGraphNode<T>
    {
        public EntityGraphNode(ITableConfig table) : this(table, null)
        {

        }

        protected EntityGraphNode(ITableConfig table, IRelationConfig relation) : base(table, relation)
        {

        }
    }

    public class EntityRelationGraphNode<T, TRelation> : EntityGraphNode<TRelation>, IEntityGraphNode<T, TRelation>
    {
        public EntityRelationGraphNode(IEntityGraphNode<T> parent, IRelationConfig<T, TRelation> relation) : base(relation.Table, relation)
        {
            this.Parent = parent;
        }

        public IEntityGraphNode<T> Parent { get; private set; }

        new public IRelationConfig<T, TRelation> Relation
        {
            get
            {
                return base.Relation as IRelationConfig<T, TRelation>;
            }
        }
    }

    public class CollectionEntityRelationGraphNode<T, TRelation> : EntityGraphNode<TRelation>, ICollectionEntityGraphNode<T, TRelation>
    {
        public CollectionEntityRelationGraphNode(IEntityGraphNode<T> parent, ICollectionRelationConfig<T, TRelation> relation) : base(relation.Table, relation)
        {
            this.Parent = parent;
        }

        public IEntityGraphNode<T> Parent { get; private set; }

        new public ICollectionRelationConfig<T, TRelation> Relation
        {
            get
            {
                return base.Relation as ICollectionRelationConfig<T, TRelation>;
            }
        }
    }
}