using System;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IRelationConfig
    {
        ITableConfig Table { get; }

        IColumnConfig Column { get; }

        RelationMultiplicity Multiplicity { get; set; }

        Type RelationType { get; }
    }

    public interface IRelationConfig<T, TRelation> : IRelationConfig
    {
        Func<T, TRelation> Getter { get; }

        Action<T, TRelation> Setter { get; }
    }

    public interface ICollectionRelationConfig<T, TRelation> : IRelationConfig
    {
        Func<T, ICollection<TRelation>> Getter { get; }

        Action<T, ICollection<TRelation>> Setter { get; }
    }

    public enum RelationMultiplicity : byte
    {
        None,
        OneToOne,
        OneToMany,
        ManyToMany
    }
}
