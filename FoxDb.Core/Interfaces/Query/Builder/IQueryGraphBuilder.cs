using System.Collections.Generic;
using System.Diagnostics;

namespace FoxDb.Interfaces
{
    public interface IQueryGraphBuilder : ICloneable<IQueryGraphBuilder>
    {
        IRelationManager RelationManager { get; }

        IQueryGraphBuilder Parent { get; set; }

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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IWithBuilder With { get; }

        ICollection<IFragmentBuilder> Fragments { get; }

        T Fragment<T>(T fragment) where T : IFragmentBuilder;

        T Fragment<T>() where T : IFragmentBuilder;

        IDatabaseQuery Build();
    }

    public interface IAggregateQueryGraphBuilder : IQueryGraphBuilder, IEnumerable<IQueryGraphBuilder>
    {

    }
}