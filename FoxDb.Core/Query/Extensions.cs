using FoxDb.Interfaces;
using System;
using System.Linq;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static IDatabaseQuerySource Source<T>(this IDatabase database, ITransactionSource transaction = null)
        {
            return database.Source<T>(null, transaction);
        }

        public static IDatabaseQuerySource Source<T>(this IDatabase database, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return database.Source(database.Config.Table<T>(), parameters, transaction);
        }

        public static IDatabaseQuerySource Source(this IDatabase database, ITableConfig table, ITransactionSource transaction = null)
        {
            return database.Source(table, null, transaction);
        }

        public static IDatabaseQuerySource Source(this IDatabase database, ITableConfig table, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            return database.Source(table, parameters, transaction);
        }

        public static IDatabaseSet<T> Set<T>(this IDatabase database, ITransactionSource transaction = null)
        {
            return database.Set<T>(database.Config.Table<T>(), null, transaction);
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

        public static IQueryGraphBuilder FetchByRelation(this IDatabase database, IRelationConfig relation)
        {
            var builder = database.QueryFactory.Build();
            var columns = relation.Expression.GetColumnMap();
            builder.Output.AddColumns(relation.RightTable.Columns);
            builder.Source.AddTable(relation.RightTable);
            switch (relation.Flags.GetMultiplicity())
            {
                case RelationFlags.OneToOne:
                case RelationFlags.OneToMany:
                    builder.Filter.AddColumn(columns[relation.RightTable].First(column => column.IsForeignKey));
                    break;
                case RelationFlags.ManyToMany:
                    builder.Source.AddRelation(relation.Invert());
                    builder.Filter.AddColumn(relation.MappingTable.LeftForeignKey);
                    break;
                default:
                    throw new NotImplementedException();
            }
            builder.Sort.AddColumns(relation.RightTable.PrimaryKeys);
            return builder;
        }
    }
}
