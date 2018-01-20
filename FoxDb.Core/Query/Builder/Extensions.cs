using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static bool GetSourceTable(this IFragmentBuilder builder, out ITableBuilder table)
        {
            switch (builder.FragmentType)
            {
                case FragmentType.Column:
                    {
                        var expression = builder as IColumnBuilder;
                        table = builder.Graph.Source.GetTable(expression.Column.Table);
                        return true;
                    }
                case FragmentType.Binary:
                    {
                        var expression = builder as IBinaryExpressionBuilder;
                        if (expression.Left.GetSourceTable(out table) || expression.Right.GetSourceTable(out table))
                        {
                            return true;
                        }
                    }
                    break;
            }
            table = default(ITableBuilder);
            return false;
        }

        public static bool GetTables(this IFragmentBuilder builder, out IEnumerable<ITableConfig> tables)
        {
            var result = new List<ITableConfig>();
            builder.GetTables(result);
            if (result.Count == 0)
            {
                tables = default(IEnumerable<ITableConfig>);
                return false;
            }
            tables = result;
            return true;
        }

        public static void GetTables(this IFragmentBuilder builder, IList<ITableConfig> tables)
        {
            switch (builder.FragmentType)
            {
                case FragmentType.Column:
                    {
                        var expression = builder as IColumnBuilder;
                        tables.Add(expression.Column.Table);
                        break;
                    }
                case FragmentType.Binary:
                    {
                        var expression = builder as IBinaryExpressionBuilder;
                        expression.Left.GetTables(tables);
                        expression.Right.GetTables(tables);
                        break;
                    }
            }
        }

        public static bool IsEmpty(this IFragmentContainer container)
        {
            return !container.Expressions.Any();
        }

        public static T GetExpression<T>(this IFragmentContainer container, Func<T, bool> predicate) where T : IFragmentBuilder
        {
            return container.Expressions.OfType<T>().FirstOrDefault(predicate);
        }

        public static IEnumerable<T> Flatten<T>(this IFragmentContainer container) where T : IFragmentBuilder
        {
            var result = new HashSet<T>();
            var stack = new Stack<IFragmentContainer>();
            stack.Push(container);
            while (stack.Count > 0)
            {
                container = stack.Pop();
                if (container is T)
                {
                    result.Add((T)container);
                }
                foreach (var expression in container.Expressions)
                {
                    if (expression is T)
                    {
                        result.Add((T)expression);
                    }
                    if (expression is IFragmentContainer)
                    {
                        stack.Push((IFragmentContainer)expression);
                    }
                }
            }
            return result;
        }

        public static IDictionary<ITableConfig, IEnumerable<IColumnConfig>> GetColumnMap(this IFragmentContainer container)
        {
            return container
                .Flatten<IColumnBuilder>()
                .GroupBy(expression => expression.Column.Table)
                .ToDictionary(group => group.Key, group => group.Select(expression => expression.Column));
        }
    }
}
