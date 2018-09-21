﻿using System.Data;
using System.Reflection;

namespace FoxDb.Interfaces
{
    public interface ITypeSelector
    {
        DbType? Type { get; }

        int? Size { get; }

        int? Precision { get; }

        int? Scale { get; }

        bool? IsNullable { get; }

        PropertyInfo Property { get; }

        TypeSelectorType SelectorType { get; }
    }

    public enum TypeSelectorType : byte
    {
        None,
        Details,
        Property
    }
}
