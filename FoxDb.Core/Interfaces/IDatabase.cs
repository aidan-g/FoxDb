﻿using System.Collections.Generic;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabase
    {
        IConfig Config { get; }

        IProvider Provider { get; }

        IDbConnection Connection { get; }

        IDbTransaction BeginTransaction();

        IDbTransaction BeginTransaction(IsolationLevel isolationLevel);

        IDatabaseSchema Schema { get; }

        IDatabaseQueryFactory QueryFactory { get; }

        IDatabaseSet<T> Set<T>(IDbTransaction transaction = null);

        IDatabaseSet<T> Query<T>(IDatabaseQuerySource source);

        void Execute(IDatabaseQuery query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null);

        void Execute(IQueryGraphBuilder query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null);

        T ExecuteScalar<T>(IDatabaseQuery query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null);

        T ExecuteScalar<T>(IQueryGraphBuilder query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null);

        T ExecuteComplex<T>(IDatabaseQuery query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null);

        T ExecuteComplex<T>(IQueryGraphBuilder query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null);

        IEnumerable<T> ExecuteEnumerator<T>(IDatabaseQuery query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null);

        IEnumerable<T> ExecuteEnumerator<T>(IQueryGraphBuilder query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null);

        IDatabaseReader ExecuteReader(IDatabaseQuery query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null);

        IDatabaseReader ExecuteReader(IQueryGraphBuilder query, DatabaseParameterHandler parameters = null, IDbTransaction transaction = null);
    }

    public delegate void DatabaseParameterHandler(IDatabaseParameters parameters);
}
