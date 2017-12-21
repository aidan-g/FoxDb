using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabase
    {
        IConfig Config { get; }

        IProvider Provider { get; }

        IDbConnection Connection { get; }

        IDatabaseQueryFactory QueryFactory { get; }

        IDatabaseSet<T> Set<T>(IDbTransaction transaction = null) where T : IPersistable;

        IDatabaseSet<T> Query<T>(IDatabaseQuery query, IDbTransaction transaction = null) where T : IPersistable;

        IDatabaseSet<T> Query<T>(IDatabaseQuery query, DatabaseParameterHandler parameters, IDbTransaction transaction = null) where T : IPersistable;

        void Execute(IDatabaseQuery query, DatabaseParameterHandler parameters, IDbTransaction transaction = null);

        T Execute<T>(IDatabaseQuery query, DatabaseParameterHandler parameters, IDbTransaction transaction = null);

        IDatabaseReader ExecuteReader(IDatabaseQuery query, DatabaseParameterHandler parameters, IDbTransaction transaction = null);
    }

    public delegate void DatabaseParameterHandler(IDatabaseParameters parameters);
}
