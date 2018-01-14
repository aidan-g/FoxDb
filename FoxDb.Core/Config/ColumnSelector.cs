using FoxDb.Interfaces;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public class ColumnSelector : IColumnSelector
    {
        public string ColumnName { get; private set; }

        public PropertyInfo Property { get; private set; }

        public Expression Expression { get; private set; }

        public ColumnFlags Flags { get; private set; }

        public ColumnSelectorType Type { get; private set; }

        public static IColumnSelector By(string columnName, ColumnFlags flags)
        {
            return new ColumnSelector()
            {
                ColumnName = columnName,
                Flags = flags,
                Type = ColumnSelectorType.ColumnName
            };
        }

        public static IColumnSelector By(PropertyInfo property, ColumnFlags flags)
        {
            return new ColumnSelector()
            {
                Property = property,
                Flags = flags,
                Type = ColumnSelectorType.Property
            };
        }

        public static IColumnSelector By(Expression expression, ColumnFlags flags)
        {
            return new ColumnSelector()
            {
                Expression = expression,
                Flags = flags,
                Type = ColumnSelectorType.Expression
            };
        }
    }
}
