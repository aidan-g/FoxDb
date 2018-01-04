using System;
using System.Collections.Generic;
using System.Reflection;

namespace FoxDb.Interfaces
{
    public interface ITableConfig
    {
        string TableName { get; set; }

        Type TableType { get; }

        IColumnConfig PrimaryKey { get; }

        IEnumerable<IColumnConfig> PrimaryKeys { get; }

        IColumnConfig ForeignKey { get; }

        IEnumerable<IColumnConfig> ForeignKeys { get; }

        IColumnConfig Column(string columnName);

        IColumnConfig Column(PropertyInfo property);

        IEnumerable<IColumnConfig> Columns { get; }

        IEnumerable<IRelationConfig> Relations { get; }
    }

    public interface ITableConfig<T> : ITableConfig
    {
        ITableConfig<T> UseDefaultColumns();

        IRelationConfig<T, TRelation> Relation<TRelation>(Func<T, TRelation> getter, Action<T, TRelation> setter, bool useDefaultColumns = true);

        ICollectionRelationConfig<T, TRelation> Relation<TRelation>(Func<T, ICollection<TRelation>> getter, Action<T, ICollection<TRelation>> setter, RelationMultiplicity multiplicity, bool useDefaultColumns = true);
    }

    public interface IMappingTableConfig : ITableConfig
    {
        ITableConfig LeftTable { get; }

        ITableConfig RightTable { get; }

        IColumnConfig LeftForeignKey { get; set; }

        IColumnConfig RightForeignKey { get; set; }
    }

    public interface ITableConfig<T1, T2> : IMappingTableConfig
    {
        ITableConfig<T1, T2> UseDefaultColumns();
    }
}
