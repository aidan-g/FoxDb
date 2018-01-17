﻿using System;

namespace FoxDb.Interfaces
{
    public interface ITableSelector
    {
        Type TableType { get; }

        ITableConfig LeftTable { get; }

        ITableConfig RightTable { get; }

        TableFlags Flags { get; }

        TableSelectorType Type { get; }
    }

    public enum TableSelectorType : byte
    {
        None,
        TableType,
        Mapping
    }
}