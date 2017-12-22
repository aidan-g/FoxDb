using System;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface ITableConfig
    {
        string Name { get; set; }

        IColumnConfig Key { get; }

        IColumnConfig Column(string name);

        IEnumerable<IColumnConfig> Columns { get; }

        IEnumerable<IRelationConfig> Relations { get; }
    }

    public interface ITableConfig<T> : ITableConfig where T : IPersistable
    {
        ITableConfig<T> UseDefaultColumns();

        IRelationConfig<T, TRelation> Relation<TRelation>(Func<T, TRelation> getter, Action<T, TRelation> setter) where TRelation : IPersistable;

        ICollectionRelationConfig<T, TRelation> Relation<TRelation>(Func<T, ICollection<TRelation>> getter, Action<T, ICollection<TRelation>> setter) where TRelation : IPersistable;
    }
}
