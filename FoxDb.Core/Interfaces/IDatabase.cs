using System.Collections.Generic;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabase
    {
        IConfig Config { get; }

        IProvider Provider { get; }

        IDbConnection Connection { get; }

        ITransactionSource BeginTransaction();

        ITransactionSource BeginTransaction(IsolationLevel isolationLevel);

        IDatabaseSchema Schema { get; }

        IDatabaseQueryFactory QueryFactory { get; }

        IDatabaseQuerySource Source<T>(ITransactionSource transaction = null);

        IDatabaseQuerySource Source(ITableConfig table, ITransactionSource transaction = null);

        IDatabaseSet<T> Set<T>(ITransactionSource transaction = null);

        IDatabaseSet<T> Query<T>(IDatabaseQuerySource source);

        void Execute(IDatabaseQuery query, DatabaseParameterHandler parameters = null, ITransactionSource transaction = null);

        void Execute(IQueryGraphBuilder query, DatabaseParameterHandler parameters = null, ITransactionSource transaction = null);

        T ExecuteScalar<T>(IDatabaseQuery query, DatabaseParameterHandler parameters = null, ITransactionSource transaction = null);

        T ExecuteScalar<T>(IQueryGraphBuilder query, DatabaseParameterHandler parameters = null, ITransactionSource transaction = null);

        T ExecuteComplex<T>(IDatabaseQuery query, DatabaseParameterHandler parameters = null, ITransactionSource transaction = null);

        T ExecuteComplex<T>(ITableConfig table, IDatabaseQuery query, DatabaseParameterHandler parameters = null, ITransactionSource transaction = null);

        T ExecuteComplex<T>(IQueryGraphBuilder query, DatabaseParameterHandler parameters = null, ITransactionSource transaction = null);

        T ExecuteComplex<T>(ITableConfig table, IQueryGraphBuilder query, DatabaseParameterHandler parameters = null, ITransactionSource transaction = null);

        IEnumerable<T> ExecuteEnumerator<T>(IDatabaseQuery query, DatabaseParameterHandler parameters = null, ITransactionSource transaction = null);

        IEnumerable<T> ExecuteEnumerator<T>(ITableConfig table, IDatabaseQuery query, DatabaseParameterHandler parameters = null, ITransactionSource transaction = null);

        IEnumerable<T> ExecuteEnumerator<T>(IQueryGraphBuilder query, DatabaseParameterHandler parameters = null, ITransactionSource transaction = null);

        IEnumerable<T> ExecuteEnumerator<T>(ITableConfig table, IQueryGraphBuilder query, DatabaseParameterHandler parameters = null, ITransactionSource transaction = null);

        IDatabaseReader ExecuteReader(IDatabaseQuery query, DatabaseParameterHandler parameters = null, ITransactionSource transaction = null);

        IDatabaseReader ExecuteReader(IQueryGraphBuilder query, DatabaseParameterHandler parameters = null, ITransactionSource transaction = null);
    }

    public delegate void DatabaseParameterHandler(IDatabaseParameters parameters);
}
