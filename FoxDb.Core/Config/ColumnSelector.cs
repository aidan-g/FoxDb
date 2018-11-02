using FoxDb.Interfaces;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public class ColumnSelector : IColumnSelector
    {
        public string Identifier { get; private set; }

        public string ColumnName { get; private set; }

        public ITypeConfig ColumnType { get; private set; }

        public PropertyInfo Property { get; private set; }

        public Expression Expression { get; private set; }

        public ColumnFlags? Flags { get; private set; }

        public ColumnSelectorType SelectorType { get; private set; }

        public static IColumnSelector By(string identifier, string columnName, ITypeConfig columnType, ColumnFlags? flags)
        {
            return new ColumnSelector()
            {
                Identifier = identifier,
                ColumnName = columnName,
                ColumnType = columnType,
                Flags = flags,
                SelectorType = ColumnSelectorType.ColumnName
            };
        }

        public static IColumnSelector By(string identifier, PropertyInfo property, ColumnFlags? flags)
        {
            return new ColumnSelector()
            {
                Identifier = identifier,
                Property = property,
                Flags = flags,
                SelectorType = ColumnSelectorType.Property
            };
        }

        public static IColumnSelector By(string identifier, Expression expression, ColumnFlags? flags)
        {
            return new ColumnSelector()
            {
                Identifier = identifier,
                Expression = expression,
                Flags = flags,
                SelectorType = ColumnSelectorType.Expression
            };
        }
    }
}
