using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabase
    {
        IConfig Config { get; }

        IProvider Provider { get; }

        IDbConnection Connection { get; }

        IDatabaseSet<T> Set<T>(IDbTransaction transaction = null) where T : IPersistable;

        IDatabaseSet<T> Query<T>(IDatabaseQuery query, IDbTransaction transaction = null) where T : IPersistable;

        IDatabaseSet<T> Query<T>(IDatabaseQuery query, DatabaseParameterHandler parameters, IDbTransaction transaction = null) where T : IPersistable;
    }

    public delegate void DatabaseParameterHandler(IDatabaseParameters parameters);
}
