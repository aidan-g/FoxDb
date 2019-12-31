using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FoxDb.Interfaces
{
    public partial interface IDatabase : IDisposable
    {
        IConfig Config { get; }

        IProvider Provider { get; }

        IDbConnection Connection { get; }

        ITransactionSource BeginTransaction();

        ITransactionSource BeginTransaction(IsolationLevel isolationLevel);

        IDatabaseTranslation Translation { get; }

        IDatabaseSchema Schema { get; }

        IDatabaseQueryCache QueryCache { get; }

        IDatabaseQueryFactory QueryFactory { get; }

        IDatabaseSchemaFactory SchemaFactory { get; }

        IDatabaseQuerySource Source(IDatabaseQueryComposer composer, DatabaseParameterHandler parameters, ITransactionSource transaction = null);

        IDatabaseSet Set(Type type, IDatabaseQuerySource source);

        IDatabaseSet<T> Set<T>(IDatabaseQuerySource source);

        int Execute(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null);

        T ExecuteScalar<T>(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null);

        IEnumerable<T> ExecuteEnumerator<T>(ITableConfig table, IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null);

        IAsyncEnumerator<T> ExecuteAsyncEnumerator<T>(ITableConfig table, IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null);

        IDatabaseReader ExecuteReader(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null);
    }

    public partial interface IDatabase
    {
        Task<int> ExecuteAsync(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null);

        Task<T> ExecuteScalarAsync<T>(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null);
    }

    public enum DatabaseParameterPhase : byte
    {
        None,
        Fetch,
        Store
    }

    public delegate void DatabaseParameterHandler(IDatabaseParameters parameters, DatabaseParameterPhase phase);
}
