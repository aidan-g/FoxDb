﻿using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IIndexSelector
    {
        string Identifier { get; }

        string IndexName { get; }

        IEnumerable<IColumnConfig> Columns { get; }

        IEnumerable<string> ColumnNames { get; }

        IndexFlags Flags { get; }

        IndexSelectorType Type { get; }
    }

    public enum IndexSelectorType : byte
    {
        None,
        Columns,
        ColumnNames
    }
}