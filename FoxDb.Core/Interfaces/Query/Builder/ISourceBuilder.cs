using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface ISourceBuilder : IFragmentContainer, IFragmentTarget
    {
        IEnumerable<ITableBuilder> Tables { get; }

        IEnumerable<ISubQueryBuilder> SubQueries { get; }

        ITableBuilder GetTable(ITableConfig table);

        ITableBuilder AddTable(ITableConfig table);

        ISubQueryBuilder GetSubQuery(IQueryGraphBuilder query);

        ISubQueryBuilder AddSubQuery(IQueryGraphBuilder query);
    }
}

