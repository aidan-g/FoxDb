using System.Diagnostics;

namespace FoxDb.Interfaces
{
    public interface ITableBuilder : IExpressionBuilder
    {
        ITableConfig Table { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IFilterBuilder Filter { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ISortBuilder Sort { get; set; }
    }
}
