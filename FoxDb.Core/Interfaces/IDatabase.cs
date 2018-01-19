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

        IDatabaseQuerySource Source(ITableConfig table, DatabaseParameterHandler parameters, ITransactionSource transaction = null);

        IDatabaseSet<T> Set<T>(IDatabaseQuerySource source);

        int Execute(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null);

        T ExecuteScalar<T>(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null);

        IDatabaseReader ExecuteReader(IDatabaseQuery query, DatabaseParameterHandler parameters, ITransactionSource transaction = null);
    }

    public delegate void DatabaseParameterHandler(IDatabaseParameters parameters);
}
