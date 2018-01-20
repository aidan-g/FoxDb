using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryFactory
    {
        IQueryGraphBuilder Build(params IQueryGraphBuilder[] queries);

        IDatabaseQuery Create(params IQueryGraph[] graphs);

        IDatabaseQuery Create(IEnumerable<IQueryGraph> graphs);

        //IDatabaseQuery Create(params IQueryGraphBuilder[] builders);

        //IDatabaseQuery Create(IEnumerable<IQueryGraphBuilder> builders);

        IDatabaseQuery Create(string commandText, params string[] parameterNames);

        IDatabaseQuery Combine(params IDatabaseQuery[] queries);

        IQueryGraphBuilder Fetch(ITableConfig table);

        IQueryGraphBuilder Add(ITableConfig table);

        IQueryGraphBuilder Update(ITableConfig table);

        IQueryGraphBuilder Delete(ITableConfig table);

        IQueryGraphBuilder Delete(ITableConfig table, IEnumerable<IColumnConfig> keys);

        IQueryGraphBuilder Count(IQueryGraphBuilder query);

        IQueryGraphBuilder Count(ITableConfig table, IQueryGraphBuilder query);
    }
}
