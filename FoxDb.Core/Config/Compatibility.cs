using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public static partial class Compatibility
    {
        [Obsolete]
        public static ITableConfig Table(this IConfig config, string tableName)
        {
            return config.Table(tableName, Defaults.Table.Flags);
        }

        [Obsolete]
        public static ITableConfig Table(this IConfig config, string tableName, TableFlags flags)
        {
            var selector = TableConfig.By(tableName, flags);
            var table = config.GetTable(selector);
            if (table == null)
            {
                table = config.CreateTable(selector);
            }
            return table;
        }

        [Obsolete]
        public static ITableConfig Table(this IConfig config, Type tableType)
        {
            return config.Table(tableType, Defaults.Table.Flags);
        }

        [Obsolete]
        public static ITableConfig Table(this IConfig config, Type tableType, TableFlags flags)
        {
            var selector = TableConfig.By(tableType, flags);
            var table = config.GetTable(selector);
            if (table == null)
            {
                table = config.CreateTable(selector);
            }
            return table;
        }

        [Obsolete]
        public static ITableConfig<T> Table<T>(this IConfig config)
        {
            return config.Table<T>(Defaults.Table.Flags);
        }

        [Obsolete]
        public static ITableConfig<T> Table<T>(this IConfig config, TableFlags flags)
        {
            return config.Table(typeof(T), flags) as ITableConfig<T>;
        }

        [Obsolete]
        public static ITableConfig<T1, T2> Table<T1, T2>(this IConfig config)
        {
            return config.Table<T1, T2>(Defaults.Table.Flags);
        }

        [Obsolete]
        public static ITableConfig<T1, T2> Table<T1, T2>(this IConfig config, TableFlags flags)
        {
            var leftTable = config.Table<T1>();
            var rightTable = config.Table<T2>();
            var selector = TableConfig.By(leftTable, rightTable, flags);
            var table = config.GetTable(selector);
            if (table == null)
            {
                table = config.CreateTable(selector);
            }
            return table as ITableConfig<T1, T2>;
        }

        [Obsolete]
        public static IRelationConfig Relation<T, TRelation>(this ITableConfig<T> table, Expression<Func<T, TRelation>> expression)
        {
            return table.Relation(expression, Defaults.Relation.Flags);
        }

        [Obsolete]
        public static IRelationConfig Relation<T, TRelation>(this ITableConfig<T> table, Expression<Func<T, TRelation>> expression, RelationFlags flags)
        {
            var selector = RelationConfig.By(expression, flags);
            var relation = table.GetRelation(selector);
            if (relation == null)
            {
                relation = table.CreateRelation<TRelation>(selector);
            }
            return relation;
        }

        [Obsolete]
        public static IColumnConfig Column(this ITableConfig table, string columnName)
        {
            return table.Column(columnName, Defaults.Column.Flags);
        }

        [Obsolete]
        public static IColumnConfig Column(this ITableConfig table, string columnName, ColumnFlags flags)
        {
            var selector = ColumnConfig.By(columnName, flags);
            var column = table.GetColumn(selector);
            if (column == null)
            {
                column = table.CreateColumn(selector);
            }
            return column;
        }

        [Obsolete]
        public static IColumnConfig Column(this ITableConfig table, PropertyInfo property)
        {
            return table.Column(property, Defaults.Column.Flags);
        }

        [Obsolete]
        public static IColumnConfig Column(this ITableConfig table, PropertyInfo property, ColumnFlags flags)
        {
            var selector = ColumnConfig.By(property, flags);
            var column = table.GetColumn(selector);
            if (column == null)
            {
                column = table.CreateColumn(selector);
            }
            return column;
        }


        [Obsolete]
        public static IEnumerable<T> ExecuteEnumerator<T>(this IDatabase database, IQueryGraphBuilder query, ITransactionSource transaction = null)
        {
            return database.ExecuteEnumerator<T>(database.Config.Table<T>(), query, null, transaction);
        }

        [Obsolete]
        public static IEnumerable<T> ExecuteEnumerator<T>(this IDatabase database, IQueryGraphBuilder query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return database.ExecuteEnumerator<T>(database.Config.Table<T>(), query.Build(), parameters, transaction);
        }

        [Obsolete]
        public static IEnumerable<T> ExecuteEnumerator<T>(this IDatabase database, IDatabaseQuery query, ITransactionSource transaction = null)
        {
            return database.ExecuteEnumerator<T>(database.Config.Table<T>(), query, null, transaction);
        }

        [Obsolete]
        public static IEnumerable<T> ExecuteEnumerator<T>(this IDatabase database, IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return database.ExecuteEnumerator<T>(database.Config.Table<T>(), query, parameters, transaction);
        }

        [Obsolete]
        public static IEnumerable<T> ExecuteEnumerator<T>(this IDatabase database, ITableConfig table, IQueryGraphBuilder query, ITransactionSource transaction = null)
        {
            return database.ExecuteEnumerator<T>(table, query, null, transaction);
        }

        [Obsolete]
        public static IEnumerable<T> ExecuteEnumerator<T>(this IDatabase database, ITableConfig table, IQueryGraphBuilder query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return database.ExecuteEnumerator<T>(table, query.Build(), parameters, transaction);
        }

        [Obsolete]
        public static IEnumerable<T> ExecuteEnumerator<T>(this IDatabase database, ITableConfig table, IDatabaseQuery query, ITransactionSource transaction = null)
        {
            return database.ExecuteEnumerator<T>(table, query, null, transaction);
        }
    }
}
