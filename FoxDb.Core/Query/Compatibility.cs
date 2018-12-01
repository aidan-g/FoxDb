using FoxDb.Interfaces;
using System;
using System.Threading.Tasks;

namespace FoxDb
{
    public static partial class Compatibility
    {
        public static IDatabaseQuerySource Source<T>(this IDatabase database, ITransactionSource transaction = null)
        {
            return database.Source<T>(null, transaction);
        }

        public static IDatabaseQuerySource Source<T>(this IDatabase database, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
#pragma warning disable 612, 618
            return database.Source(database.Config.Table<T>(), parameters, transaction);
#pragma warning restore 612, 618
        }

        public static IDatabaseQuerySource Source(this IDatabase database, ITableConfig table, ITransactionSource transaction = null)
        {
            return database.Source(table, null, transaction);
        }

        public static IDatabaseQuerySource Source(this IDatabase database, ITableConfig table, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return database.Source(new EntityRelationQueryComposer(database, table), parameters, transaction);
        }

        public static IDatabaseQuerySource Source(this IDatabase database, IDatabaseQueryComposer composer, ITransactionSource transaction = null)
        {
            return database.Source(composer, null, transaction);
        }

        public static IDatabaseQuerySource Source(this IDatabase database, IDatabaseQueryComposer composer, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return database.Source(composer, parameters, transaction);
        }

        public static IDatabaseSet Set(this IDatabase database, Type tableType, ITransactionSource transaction = null)
        {
#pragma warning disable 612, 618
            return database.Set(database.Config.Table(tableType), null, transaction);
#pragma warning restore 612, 618
        }

        public static IDatabaseSet Set(this IDatabase database, ITableConfig table, ITransactionSource transaction = null)
        {
            return database.Set(table, null, transaction);
        }

        public static IDatabaseSet Set(this IDatabase database, ITableConfig table, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return database.Set(table.TableType, database.Source(table, parameters, transaction));
        }

        public static IDatabaseSet<T> Set<T>(this IDatabase database, ITransactionSource transaction = null)
        {
#pragma warning disable 612, 618
            return database.Set<T>(database.Config.Table<T>(), null, transaction);
#pragma warning restore 612, 618
        }

        public static IDatabaseSet<T> Set<T>(this IDatabase database, ITableConfig table, ITransactionSource transaction = null)
        {
            return database.Set<T>(table, null, transaction);
        }

        public static IDatabaseSet<T> Set<T>(this IDatabase database, ITableConfig table, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return database.Set<T>(database.Source(table, parameters, transaction));
        }

        public static int Execute(this IDatabase database, IQueryGraphBuilder query, ITransactionSource transaction = null)
        {
            return database.Execute(query, null, transaction);
        }

        public static int Execute(this IDatabase database, IQueryGraphBuilder query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return database.Execute(query.Build(), parameters, transaction);
        }

        public static int Execute(this IDatabase database, IDatabaseQuery query, ITransactionSource transaction = null)
        {
            return database.Execute(query, null, transaction);
        }

        public static Task<int> ExecuteAsync(this IDatabase database, IQueryGraphBuilder query, ITransactionSource transaction = null)
        {
            return database.ExecuteAsync(query, null, transaction);
        }

        public static Task<int> ExecuteAsync(this IDatabase database, IQueryGraphBuilder query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return database.ExecuteAsync(query.Build(), parameters, transaction);
        }

        public static Task<int> ExecuteAsync(this IDatabase database, IDatabaseQuery query, ITransactionSource transaction = null)
        {
            return database.ExecuteAsync(query, null, transaction);
        }

        public static T ExecuteScalar<T>(this IDatabase database, IQueryGraphBuilder query, ITransactionSource transaction = null)
        {
            return database.ExecuteScalar<T>(query, null, transaction);
        }

        public static T ExecuteScalar<T>(this IDatabase database, IQueryGraphBuilder query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return database.ExecuteScalar<T>(query.Build(), parameters, transaction);
        }

        public static T ExecuteScalar<T>(this IDatabase database, IDatabaseQuery query, ITransactionSource transaction = null)
        {
            return database.ExecuteScalar<T>(query, null, transaction);
        }

        public static Task<T> ExecuteScalarAsync<T>(this IDatabase database, IQueryGraphBuilder query, ITransactionSource transaction = null)
        {
            return database.ExecuteScalarAsync<T>(query, null, transaction);
        }

        public static Task<T> ExecuteScalarAsync<T>(this IDatabase database, IQueryGraphBuilder query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return database.ExecuteScalarAsync<T>(query.Build(), parameters, transaction);
        }

        public static Task<T> ExecuteScalarAsync<T>(this IDatabase database, IDatabaseQuery query, ITransactionSource transaction = null)
        {
            return database.ExecuteScalarAsync<T>(query, null, transaction);
        }

        public static IDatabaseReader ExecuteReader(this IDatabase database, IQueryGraphBuilder query, ITransactionSource transaction = null)
        {
            return database.ExecuteReader(query, null, transaction);
        }

        public static IDatabaseReader ExecuteReader(this IDatabase database, IQueryGraphBuilder query, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return database.ExecuteReader(query.Build(), parameters, transaction);
        }

        public static IDatabaseReader ExecuteReader(this IDatabase database, IDatabaseQuery query, ITransactionSource transaction = null)
        {
            return database.ExecuteReader(query, null, transaction);
        }
    }
}
