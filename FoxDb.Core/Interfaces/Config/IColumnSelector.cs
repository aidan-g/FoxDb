using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb.Interfaces
{
    public interface IColumnSelector
    {
        string Identifier { get; }

        string ColumnName { get; }

        ITypeConfig ColumnType { get; }

        PropertyInfo Property { get; }

        Expression Expression { get; }

        ColumnFlags? Flags { get; }

        ColumnSelectorType SelectorType { get; }
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
