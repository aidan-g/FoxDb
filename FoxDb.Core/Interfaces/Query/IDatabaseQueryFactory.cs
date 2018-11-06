using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryFactory
    {
        IDatabaseQueryDialect Dialect { get; }

        IQueryGraphBuilder Build();

        IDatabaseQuery Create(IQueryGraphBuilder graph);

        IDatabaseQuery Create(string commandText, params IDatabaseQueryParameter[] parameters);

        IQueryGraphBuilder Combine(IEnumerable<IQueryGraphBuilder> graphs);

        IDatabaseQuery Combine(IEnumerable<IDatabaseQuery> queries);

        IQueryGraphBuilder Exists(ITableConfig table);

        IQueryGraphBuilder Fetch(ITableConfig table);

        IQueryGraphBuilder Lookup(ITableConfig table);

        IQueryGraphBuilder Lookup(IRelationConfig relation);

        IQueryGraphBuilder Add(ITableConfig table);

        IQueryGraphBuilder Update(ITableConfig table);

        IQueryGraphBuilder Delete(ITableConfig table);

        IQueryGraphBuilder Count(IQueryGraphBuilder query);

        IQueryGraphBuilder Count(ITableConfig table, IQueryGraphBuilder query);
    }
}
