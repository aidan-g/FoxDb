using System.Collections.Generic;
using System.Diagnostics;

namespace FoxDb.Interfaces
{
    public interface ISchemaGraphBuilder : ICloneable<ISchemaGraphBuilder>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ICreateBuilder Create { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IAlterBuilder Alter { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IDropBuilder Drop { get; }

        ICollection<IFragmentBuilder> Fragments { get; }

        T Fragment<T>(T fragment) where T : IFragmentBuilder;

        T Fragment<T>() where T : IFragmentBuilder;

        IDatabaseQuery Build();
    }

    public interface IAggregateSchemaGraphBuilder : ISchemaGraphBuilder, IEnumerable<ISchemaGraphBuilder>
    {

    }
}
