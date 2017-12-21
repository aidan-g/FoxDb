using System;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface ITableConfig
    {
        string Name { get; set; }

        void UseDefaultColumns();

        IColumnConfig Key { get; }

        IColumnConfig Column(string name);

        IEnumerable<IColumnConfig> Columns { get; }

        IEnumerable<IRelationConfig> Relations { get; }
    }

    public interface ITableConfig<T> : ITableConfig
    {
        IRelationConfig<T, TRelation> Relation<TRelation>(string name, Func<T, TRelation> selector);

        ICollectionRelationConfig<T, TRelation> Relation<TRelation>(string name, Func<T, ICollection<TRelation>> selector);
    }
}
