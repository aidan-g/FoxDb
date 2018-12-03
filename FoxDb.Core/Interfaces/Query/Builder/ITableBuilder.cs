using System;
using System.Diagnostics;

namespace FoxDb.Interfaces
{
    public interface ITableBuilder : IFragmentContainer, IFragmentTarget
    {
        ITableConfig Table { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IFilterBuilder Filter { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ISortBuilder Sort { get; set; }

        LockHints LockHints { get; set; }
    }

    [Flags]
    public enum LockHints : byte
    {
        None = 0,
        RowLock = 1,
        PageLock = 2,
        TableLock = 4,
        DatabaseLock = 8,
        UpdateLock = 16,
        ExclusiveLock = 32,
        HoldLock = 64,
        NoLock = 128
    }
}
