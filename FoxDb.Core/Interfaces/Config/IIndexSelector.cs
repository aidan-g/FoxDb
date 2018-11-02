using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IIndexSelector
    {
        string Identifier { get; }

        IEnumerable<IColumnConfig> Columns { get; }

        IEnumerable<string> ColumnNames { get; }

        IndexFlags? Flags { get; }

        IndexSelectorType SelectorType { get; }
    }

    public enum IndexSelectorType : byte
    {
        None,
        Columns,
        ColumnNames
    }
}
