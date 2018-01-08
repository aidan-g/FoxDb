﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb.Interfaces
{
    public interface ITableConfig
    {
        IConfig Config { get; }

        string TableName { get; set; }

        Type TableType { get; }

        IColumnConfig PrimaryKey { get; }

        IEnumerable<IColumnConfig> PrimaryKeys { get; }

        IColumnConfig ForeignKey { get; }

        IEnumerable<IColumnConfig> ForeignKeys { get; }

        IColumnConfig Column(string name);

        IColumnConfig Column(PropertyInfo property);

        IEnumerable<IColumnConfig> Columns { get; }

        IEnumerable<IRelationConfig> Relations { get; }
    }

    public interface ITableConfig<T> : ITableConfig
    {
        ITableConfig<T> UseDefaultColumns();

        ITableConfig<T> UseDefaultRelations();

        IRelationConfig<T, TRelation> Relation<TRelation>(Expression<Func<T, TRelation>> expression);

        ICollectionRelationConfig<T, TRelation> Relation<TRelation>(Expression<Func<T, IEnumerable<TRelation>>> expression, RelationMultiplicity multiplicity);
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
