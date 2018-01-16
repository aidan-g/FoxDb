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

        IQueryGraphBuilder Fetch(ITableConfig table);

        IEnumerable<IQueryGraphBuilder> Add(ITableConfig table);

        IQueryGraphBuilder Update(ITableConfig table);

        IQueryGraphBuilder Delete(ITableConfig table);

        IQueryGraphBuilder Delete(ITableConfig table, IEnumerable<IColumnConfig> keys);

        IQueryGraphBuilder Count(IQueryGraphBuilder query);
    }
}
