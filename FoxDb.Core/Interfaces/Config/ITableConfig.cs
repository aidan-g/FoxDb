using System;
using System.Collections.Generic;

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

        IEnumerable<IColumnConfig> Columns { get; }

        IEnumerable<IRelationConfig> Relations { get; }
    }

    public interface ITableConfig<T> : ITableConfig where T : IPersistable
    {
        ITableConfig<T> UseDefaultColumns();

        IRelationConfig<T, TRelation> Relation<TRelation>(Func<T, TRelation> getter, Action<T, TRelation> setter, bool useDefaultColumns = true) where TRelation : IPersistable;

        ICollectionRelationConfig<T, TRelation> Relation<TRelation>(Func<T, ICollection<TRelation>> getter, Action<T, ICollection<TRelation>> setter, bool useDefaultColumns = true) where TRelation : IPersistable;
    }

    public interface ITableConfig<T1, T2> : ITableConfig where T1 : IPersistable where T2 : IPersistable
    {
        IColumnConfig LeftForeignKey { get; set; }

        IColumnConfig RightForeignKey { get; set; }

        ITableConfig<T1, T2> UseDefaultColumns();
    }
}
