using System;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface ITableConfig : IEquatable<ITableConfig>
    {
        IConfig Config { get; }

        TableFlags Flags { get; }

        string TableName { get; set; }

        Type TableType { get; }

        IColumnConfig PrimaryKey { get; }

        IEnumerable<IColumnConfig> PrimaryKeys { get; }

        IColumnConfig ForeignKey { get; }

        IEnumerable<IColumnConfig> ForeignKeys { get; }

        IColumnConfig GetColumn(IColumnSelector selector);

        IColumnConfig CreateColumn(IColumnSelector selector);

        bool TryCreateColumn(IColumnSelector selector, out IColumnConfig column);

        IEnumerable<IColumnConfig> Columns { get; }

        IEnumerable<IRelationConfig> Relations { get; }

        ITableConfig AutoColumns();

        ITableConfig AutoRelations();

        ITableConfig<T> CreateProxy<T>();
    }

    public interface ITableConfig<T> : ITableConfig
    {
        IRelationConfig GetRelation<TRelation>(IRelationSelector<T, TRelation> selector);

        IRelationConfig CreateRelation<TRelation>(IRelationSelector<T, TRelation> selector);

        bool TryCreateRelation<TRelation>(IRelationSelector<T, TRelation> selector, out IRelationConfig relation);
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

    }
}
