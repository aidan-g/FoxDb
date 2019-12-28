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
            var column = builder as IColumnBuilder;
            var container = builder as IFragmentContainer;
            if (column != null)
            {
                table = builder.Graph.Source.GetTable(column.Column.Table);
            }
            else if (container != null)
            {
                table = container.GetTables().Select(builder.Graph.Source.GetTable).FirstOrDefault();
            }
            else
            {
                table = default(ITableBuilder);
                return false;
            }
            return table != null;
        }

        public static IEnumerable<ITableConfig> GetTables(this IFragmentContainer container)
        {
            return container
                .Flatten<IColumnBuilder>()
                .Select(expression => expression.Column.Table)
                .Distinct();
        }

        public static IEnumerable<IColumnConfig> GetColumns(this IFragmentContainer container)
        {
            return container
                .Flatten<IColumnBuilder>()
                .Select(expression => expression.Column)
                .Distinct();
        }

        public static IDictionary<ITableConfig, IList<IColumnConfig>> GetColumnMap(this IFragmentContainer container)
        {
            return container
                .Flatten<IColumnBuilder>()
                .GroupBy(expression => expression.Column.Table)
                .ToDictionary(group => group.Key, group => (IList<IColumnConfig>)group.Select(expression => expression.Column).ToList());
        }

        public static TResult GetOppositeExpression<T, TResult>(this IFragmentContainer container, Predicate<T> predicate) where T : IFragmentBuilder
        {
            var expressions = container.Flatten<IBinaryExpressionBuilder>();
            foreach (var binary in expressions)
            {
                if (binary.Left is T && predicate((T)binary.Left) && binary.Right is TResult)
                {
                    return (TResult)binary.Right;
                }
                if (binary.Right is T && predicate((T)binary.Right) && binary.Left is TResult)
                {
                    return (TResult)binary.Left;
                }
            }
            return default(TResult);
        }

        public static bool IsEmpty(this IFragmentBuilder builder)
        {
            if (builder is IFragmentContainer)
            {
                return !(builder as IFragmentContainer).Expressions.Any(expression => expression != null);
            }
            else
            {
                return false;
            }
        }

        public static T GetExpression<T>(this IFragmentContainer container) where T : IFragmentBuilder
        {
            return container.Expressions.OfType<T>().FirstOrDefault();
        }

        public static T GetExpression<T>(this IFragmentContainer container, Func<T, bool> predicate) where T : IFragmentBuilder
        {
            return container.Expressions.OfType<T>().FirstOrDefault(predicate);
        }

        public static IEnumerable<T> GetExpressions<T>(this IFragmentContainer container, Func<T, bool> predicate) where T : IFragmentBuilder
        {
            return container.Expressions.OfType<T>().Where(predicate);
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
    }
}
