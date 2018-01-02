using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabase
    {
        IConfig Config { get; }

        IProvider Provider { get; }

        IDbConnection Connection { get; }

        IDatabaseSchema Schema { get; }

        IDatabaseQueryFactory QueryFactory { get; }

        IDatabaseSet<T> Set<T>(IDbTransaction transaction = null);

        IDatabaseSet<T> Set<T>(bool includeRelations, IDbTransaction transaction = null);

        IDatabaseSet<T> Query<T>(IDatabaseQuerySource source);

        void Execute(IDatabaseQuery query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null);

        void Execute(IQueryGraphBuilder query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null);

        T Execute<T>(IDatabaseQuery query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null);

        T Execute<T>(IQueryGraphBuilder query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null);

        IDatabaseReader ExecuteReader(IDatabaseQuery query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null);

        IDatabaseReader ExecuteReader(IQueryGraphBuilder query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null);
    }

    public delegate void DatabaseParameterHandler(IDatabaseParameters parameters);
}
