using System;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IRelationConfig
    {
        ITableConfig Parent { get; }

        ITableConfig Table { get; }

        IColumnConfig Column { get; }

        RelationBehaviour Behaviour { get; set; }

        RelationMultiplicity Multiplicity { get; set; }

        Type RelationType { get; }

        IRelationConfig With(Action<IRelationConfig> relation);
    }

    public interface IRelationConfig<T, TRelation> : IRelationConfig
    {
        new ITableConfig<T> Parent { get; }

        new ITableConfig<TRelation> Table { get; }

        Func<T, TRelation> Getter { get; }

        Action<T, TRelation> Setter { get; }

        IRelationConfig<T, TRelation> With(Action<IRelationConfig<T, TRelation>> relation);
    }

    public interface ICollectionRelationConfig<T, TRelation> : IRelationConfig
    {
        new ITableConfig<T> Parent { get; }

        new ITableConfig<TRelation> Table { get; }

        Func<T, ICollection<TRelation>> Getter { get; }

        Action<T, ICollection<TRelation>> Setter { get; }

        ICollectionRelationConfig<T, TRelation> With(Action<ICollectionRelationConfig<T, TRelation>> relation);
    }

    [Flags]
    public enum RelationBehaviour : byte
    {
        None = 0,
        EagerFetch = 1
    }

    public enum RelationMultiplicity : byte
    {
        None,
        OneToOne,
        OneToMany,
        ManyToMany
    }
}
