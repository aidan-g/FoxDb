using System.Collections.Generic;
using System.Diagnostics;

namespace FoxDb.Interfaces
{
    public interface IQueryGraphBuilder
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IOutputBuilder Output { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IAddBuilder Add { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IUpdateBuilder Update { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IDeleteBuilder Delete { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ISourceBuilder Source { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IFilterBuilder Filter { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IAggregateBuilder Aggregate { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ISortBuilder Sort { get; }

        T Fragment<T>() where T : IFragmentBuilder;

        IEnumerable<IQueryGraph> Build();

        IQueryGraphBuilder Clone();
    }

    public interface IAggregateQueryGraphBuilder : IQueryGraphBuilder, IEnumerable<IQueryGraphBuilder>
    {

    }
}