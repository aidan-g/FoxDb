using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IEntityGraphNode
    {
        ITableConfig Table { get; }

        IRelationConfig Relation { get; }

        IEnumerable<IEntityGraphNode> Children { get; }
    }

    public interface IEntityGraphNode<T> : IEntityGraphNode
    {

    }

    public interface IEntityGraphNode<T, TRelation> : IEntityGraphNode<T>
    {
        IEntityGraphNode<T> Parent { get; }

        new IRelationConfig<T, TRelation> Relation { get; }
    }

    public interface ICollectionEntityGraphNode<T, TRelation> : IEntityGraphNode<T>
    {
        IEntityGraphNode<T> Parent { get; }

        new ICollectionRelationConfig<T, TRelation> Relation { get; }
    }
}
