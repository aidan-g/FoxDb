﻿using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryFactory
    {
        IQueryGraphBuilder Build();

        IDatabaseQuery Create(IQueryGraphBuilder graph);

        IDatabaseQuery Create(string commandText, params string[] parameterNames);

        IQueryGraphBuilder Combine(IEnumerable<IQueryGraphBuilder> graphs);

        IDatabaseQuery Combine(IEnumerable<IDatabaseQuery> queries);

        IQueryGraphBuilder Fetch(ITableConfig table);

        IQueryGraphBuilder Add(ITableConfig table);

        IQueryGraphBuilder Update(ITableConfig table);

        IQueryGraphBuilder Delete(ITableConfig table);

        IQueryGraphBuilder Delete(ITableConfig table, IEnumerable<IColumnConfig> keys);

        IQueryGraphBuilder Count(IQueryGraphBuilder query);

        IQueryGraphBuilder Count(ITableConfig table, IQueryGraphBuilder query);
    }
}
