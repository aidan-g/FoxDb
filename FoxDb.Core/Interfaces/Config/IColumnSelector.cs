using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb.Interfaces
{
    public interface IColumnSelector
    {
        string ColumnName { get; }

        PropertyInfo Property { get; }

        Expression Expression { get; }

        ColumnFlags Flags { get; }

        ColumnSelectorType Type { get; }
    }

    public interface IColumnSelector<T, TRelation> : IColumnSelector
    {
        new Expression<Func<T, TRelation>> Expression { get; }
    }

    public enum ColumnSelectorType : byte
    {
        None,
        ColumnName,
        Property,
        Expression
    }
}
