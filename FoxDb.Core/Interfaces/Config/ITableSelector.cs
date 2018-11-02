using System;

namespace FoxDb.Interfaces
{
    public interface ITableSelector
    {
        string Identifier { get; }

        string TableName { get; }

        Type TableType { get; }

        ITableConfig LeftTable { get; }

        ITableConfig RightTable { get; }

        TableFlags? Flags { get; }

        TableSelectorType SelectorType { get; }
    }

    public enum TableSelectorType : byte
    {
        None,
        TableName,
        TableType,
        Mapping
    }
}
