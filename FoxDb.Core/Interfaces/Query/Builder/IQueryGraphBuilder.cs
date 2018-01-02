using System.Diagnostics;

namespace FoxDb.Interfaces
{
    public interface IQueryGraphBuilder
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ISelectBuilder Select { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IInsertBuilder Insert { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IUpdateBuilder Update { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IDeleteBuilder Delete { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IFromBuilder From { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IWhereBuilder Where { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IOrderByBuilder OrderBy { get; }

        T Fragment<T>() where T : IFragmentBuilder;

        IQueryGraph Build();
    }
}