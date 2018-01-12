using System;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IEntityGraphNode
    {
        Type EntityType { get; }

        IEntityGraphNode Parent { get; }

        ITableConfig Table { get; }

        IRelationConfig Relation { get; }

        IEnumerable<IEntityGraphNode> Children { get; }
    }

    public interface IEntityGraphNode<T> : IEntityGraphNode
    {

    }

    public interface IEntityGraphNode<T, TRelation> : IEntityGraphNode<T>
    {
        new IRelationConfig<T, TRelation> Relation { get; }
    }

    public interface ICollectionEntityGraphNode<T, TRelation> : IEntityGraphNode<T>
    {
        new ICollectionRelationConfig<T, TRelation> Relation { get; }
    }
}
