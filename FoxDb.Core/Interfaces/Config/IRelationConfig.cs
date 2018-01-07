using System;
using System.Collections.Generic;
using System.Reflection;

namespace FoxDb.Interfaces
{
    public interface IRelationConfig
    {
        IConfig Config { get; }

        ITableConfig LeftTable { get; }

        IMappingTableConfig MappingTable { get; }

        ITableConfig RightTable { get; }

        PropertyInfo Property { get; }

        IColumnConfig LeftColumn { get; }

        IColumnConfig RightColumn { get; }

        RelationBehaviour Behaviour { get; set; }

        RelationMultiplicity Multiplicity { get; }

        Type RelationType { get; }

        bool Inverted { get; set; }

        IRelationConfig Invert();
    }

    public interface IRelationConfig<T, TRelation> : IRelationConfig
    {
        Func<T, TRelation> Getter { get; }

        Action<T, TRelation> Setter { get; }

        IRelationConfig<T, TRelation> UseDefaultColumns();
    }

    public interface ICollectionRelationConfig<T, TRelation> : IRelationConfig
    {
        Func<T, ICollection<TRelation>> Getter { get; }

        Action<T, ICollection<TRelation>> Setter { get; }

        ICollectionRelationConfig<T, TRelation> UseDefaultColumns();
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
