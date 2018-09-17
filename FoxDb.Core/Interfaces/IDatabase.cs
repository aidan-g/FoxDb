using System;
using System.Collections.Generic;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabase : IDisposable
    {
        IConfig Config { get; }

        IProvider Provider { get; }

        IDbConnection Connection { get; }

        ITransactionSource BeginTransaction();

        ITransactionSource BeginTransaction(IsolationLevel isolationLevel);

        IDatabaseSchema Schema { get; }

        IDatabaseQueryCache QueryCache { get; }

        IDatabaseQueryFactory QueryFactory { get; }

        IDatabaseSchemaFactory SchemaFactory { get; }

        IDatabaseQuerySource Source(IDatabaseQueryComposer composer, DatabaseParameterHandler parameters, ITransactionSource transaction = null);

        IDatabaseSet<T> Set<T>(IDatabaseQuerySource source);

        int Execute(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null);

        T ExecuteScalar<T>(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null);

        [Obsolete]
        IEnumerable<T> ExecuteEnumerator<T>(ITableConfig table, IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null);

        IDatabaseReader ExecuteReader(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null);
    }

    public delegate void DatabaseParameterHandler(IDatabaseParameters parameters);
}
