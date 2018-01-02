using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryFactory
    {
        IQueryGraphBuilder Build();

        IDatabaseQuery Create(params IQueryGraph[] graphs);

        IDatabaseQuery Create(IEnumerable<IQueryGraph> graphs);

        IDatabaseQuery Create(params IQueryGraphBuilder[] builders);

        IDatabaseQuery Create(IEnumerable<IQueryGraphBuilder> builders);

        IDatabaseQuery Create(string commandText, params string[] parameterNames);

        IQueryGraphBuilder Select<T>();

        IEnumerable<IQueryGraphBuilder> Insert<T>();

        IEnumerable<IQueryGraphBuilder> Insert<T1, T2>();

        IQueryGraphBuilder Update<T>();

        IQueryGraphBuilder Delete<T>();

        IQueryGraphBuilder Delete<T1, T2>();

        IQueryGraphBuilder Count(IQueryGraphBuilder query);
    }
}
