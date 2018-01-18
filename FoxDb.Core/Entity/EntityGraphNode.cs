using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public abstract class EntityGraphNode : IEntityGraphNode
    {
        public EntityGraphNode(Type entityType, IEntityGraphNode parent, ITableConfig table, IRelationConfig relation)
        {
            this.EntityType = entityType;
            this.Parent = parent;
            this.Table = table;
            this.Relation = relation;
        }

        public Type EntityType { get; private set; }

        public IEntityGraphNode Parent { get; private set; }

        public ITableConfig Table { get; private set; }

        public IRelationConfig Relation { get; private set; }

        public IEnumerable<IEntityGraphNode> Children { get; set; }
    }

    public class EntityGraphNode<T> : EntityGraphNode, IEntityGraphNode<T>
    {
        public EntityGraphNode(ITableConfig table) : this(null, table, null)
        {

        }

        protected EntityGraphNode(IEntityGraphNode parent, ITableConfig table, IRelationConfig relation) : base(typeof(T), parent, table, relation)
        {

        }
    }

    public class EntityRelationGraphNode<T, TRelation> : EntityGraphNode<TRelation>, IEntityGraphNode<T, TRelation>
    {
        public EntityRelationGraphNode(IEntityGraphNode parent, IRelationConfig<T, TRelation> relation) : base(parent, relation.RightTable, relation)
        {

        }

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
        public CollectionEntityRelationGraphNode(IEntityGraphNode parent, ICollectionRelationConfig<T, TRelation> relation) : base(parent, relation.RightTable, relation)
        {

        }

        new public ICollectionRelationConfig<T, TRelation> Relation
        {
            get
            {
                return base.Relation as ICollectionRelationConfig<T, TRelation>;
            }
        }
    }
}