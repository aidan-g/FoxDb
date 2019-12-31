#pragma warning disable 612, 618
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
        public static ITableConfig Table(this IConfig config, string tableName, TableFlags? flags = null)
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
        public static ITableConfig Table(this IConfig config, Type tableType, TableFlags? flags = null)
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
        public static ITableConfig<T> Table<T>(this IConfig config, TableFlags? flags = null)
        {
            return config.Table(typeof(T), flags) as ITableConfig<T>;
        }

        [Obsolete]
        public static ITableConfig<T1, T2> Table<T1, T2>(this IConfig config, TableFlags? flags = null)
        {
            var leftTable = config.Table<T1>(flags);
            var rightTable = config.Table<T2>(flags);
            var selector = TableConfig.By(leftTable, rightTable, flags);
            var table = config.GetTable(selector);
            if (table == null)
            {
                table = config.CreateTable(selector);
            }
            return table as ITableConfig<T1, T2>;
        }

        [Obsolete]
        public static IIndexConfig Index(this ITableConfig table, IEnumerable<string> columnNames, IndexFlags? flags = null)
        {
            var selector = IndexConfig.By(columnNames, flags);
            var index = table.GetIndex(selector);
            if (index == null)
            {
                index = table.CreateIndex(selector);
            }
            return index;
        }

        [Obsolete]
        public static IIndexConfig Index(this ITableConfig table, IEnumerable<IColumnConfig> columns, IndexFlags? flags = null)
        {
            var selector = IndexConfig.By(columns, flags);
            var index = table.GetIndex(selector);
            if (index == null)
            {
                index = table.CreateIndex(selector);
            }
            return index;
        }

        [Obsolete]
        public static IRelationConfig Relation<T, TRelation>(this ITableConfig<T> table, Expression<Func<T, TRelation>> expression, RelationFlags? flags = null)
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
        public static IRelationConfig Relation<T1, T2, TRelation>(this ITableConfig<T1, T2> table, Expression<Func<T1, TRelation>> expression, RelationFlags? flags = null)
        {
            var selector = RelationConfig.By(expression, flags);
            var relation = table.GetRelation(selector);
            if (relation == null)
            {
                relation = table.CreateRelation(selector);
            }
            return relation;
        }

        [Obsolete]
        public static IColumnConfig Column(this ITableConfig table, string columnName, ColumnFlags? flags = null)
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
        public static IColumnConfig Column(this ITableConfig table, PropertyInfo property, ColumnFlags? flags = null)
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

        [Obsolete]
        public static IAsyncEnumerator<T> ExecuteAsyncEnumerator<T>(this IDatabase database, IQueryGraphBuilder query, ITransactionSource transaction = null)
        {
            return database.ExecuteAsyncEnumerator<T>(database.Config.Table<T>(), query, null, transaction);
        }

        [Obsolete]
        public static IAsyncEnumerator<T> ExecuteAsyncEnumerator<T>(this IDatabase database, IQueryGraphBuilder query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return database.ExecuteAsyncEnumerator<T>(database.Config.Table<T>(), query.Build(), parameters, transaction);
        }

        [Obsolete]
        public static IAsyncEnumerator<T> ExecuteAsyncEnumerator<T>(this IDatabase database, IDatabaseQuery query, ITransactionSource transaction = null)
        {
            return database.ExecuteAsyncEnumerator<T>(database.Config.Table<T>(), query, null, transaction);
        }

        [Obsolete]
        public static IAsyncEnumerator<T> ExecuteAsyncEnumerator<T>(this IDatabase database, IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return database.ExecuteAsyncEnumerator<T>(database.Config.Table<T>(), query, parameters, transaction);
        }

        [Obsolete]
        public static IAsyncEnumerator<T> ExecuteAsyncEnumerator<T>(this IDatabase database, ITableConfig table, IQueryGraphBuilder query, ITransactionSource transaction = null)
        {
            return database.ExecuteAsyncEnumerator<T>(table, query, null, transaction);
        }

        [Obsolete]
        public static IAsyncEnumerator<T> ExecuteAsyncEnumerator<T>(this IDatabase database, ITableConfig table, IQueryGraphBuilder query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return database.ExecuteAsyncEnumerator<T>(table, query.Build(), parameters, transaction);
        }

        [Obsolete]
        public static IAsyncEnumerator<T> ExecuteAsyncEnumerator<T>(this IDatabase database, ITableConfig table, IDatabaseQuery query, ITransactionSource transaction = null)
        {
            return database.ExecuteAsyncEnumerator<T>(table, query, null, transaction);
        }
    }
}
