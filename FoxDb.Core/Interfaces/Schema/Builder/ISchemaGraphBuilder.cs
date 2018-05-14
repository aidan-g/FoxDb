using System.Diagnostics;

namespace FoxDb.Interfaces
{
    public interface ISchemaGraphBuilder
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ICreateBuilder Create { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IAlterBuilder Alter { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IDropBuilder Drop { get; }
    }
}
