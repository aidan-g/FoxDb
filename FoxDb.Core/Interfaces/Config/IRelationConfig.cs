using System;
using System.Collections.Generic;
using System.Reflection;

namespace FoxDb.Interfaces
{
    public interface IRelationConfig : IEquatable<IRelationConfig>
    {
        IConfig Config { get; }

        RelationFlags Flags { get; }

        ITableConfig LeftTable { get; }

        IMappingTableConfig MappingTable { get; }

        ITableConfig RightTable { get; }

        PropertyInfo Property { get; }

        IColumnConfig LeftColumn { get; }

        IColumnConfig RightColumn { get; }

        Type RelationType { get; }

        IRelationConfig Invert();
    }

    public interface IRelationConfig<in T, TRelation> : IRelationConfig
    {
        Func<T, TRelation> Getter { get; }

        Action<T, TRelation> Setter { get; }
    }

    public interface ICollectionRelationConfig<in T, TRelation> : IRelationConfig
    {
        Func<T, ICollection<TRelation>> Getter { get; }

        Action<T, ICollection<TRelation>> Setter { get; }
    }
}
