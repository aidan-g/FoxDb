using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static T CreateDelegate<T>(this MethodInfo method)
        {
            return (T)(object)method.CreateDelegate(typeof(T));
        }

        public static PropertyInfo GetLambdaProperty<T>(this Expression expression)
        {
            return expression.GetLambdaProperty(typeof(T));
        }

        public static PropertyInfo GetLambdaProperty(this Expression expression, ITableConfig table)
        {
            if (table is IMappingTableConfig)
            {
                return expression.GetLambdaProperty((table as IMappingTableConfig).LeftTable.TableType);
            }
            return expression.GetLambdaProperty(table.TableType);
        }

        public static PropertyInfo GetLambdaProperty(this Expression expression, Type type)
        {
            if (expression.NodeType != ExpressionType.Lambda)
            {
                throw new NotImplementedException();
            }
            var lambda = expression as LambdaExpression;
            if (lambda.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new NotImplementedException();
            }
            var member = lambda.Body as MemberExpression;
            if (!(member.Member is PropertyInfo))
            {
                throw new NotImplementedException();
            }
            var property = member.Member as PropertyInfo;
            if (property.DeclaringType == type)
            {
                return property;
            }
            return PropertyResolutionStrategy.GetProperty(type, property.Name);
        }

        public static IEnumerable<TResult> SelectMany<T, TResult>(this IEnumerable<T> sequence) where T : IEnumerable<TResult>
        {
            return sequence.SelectMany(element => element);
        }

        public static bool Contains<T>(this IEnumerable<T> sequence1, IEnumerable<T> sequence2)
        {
            //TODO: Why the fuck does this not work?
            //return sequence1.Intersect(sequence2).Count() == values.Count();
            var count = 0;
            foreach (var element in sequence2)
            {
                if (sequence1.Contains(element))
                {
                    count++;
                }
            }
            return count == sequence2.Count();
        }

        public static bool Contains<T>(this IEnumerable<T> sequence1, IEnumerable<T> sequence2, IEqualityComparer<T> comparer)
        {
            //TODO: Why the fuck does this not work?
            //return sequence1.Intersect(sequence2, comparer).Count() == values.Count();
            var count = 0;
            foreach (var element in sequence2)
            {
                if (sequence1.Contains(element, comparer))
                {
                    count++;
                }
            }
            return count == sequence2.Count();
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> sequence1, IEnumerable<IEnumerable<T>> sequence2)
        {
            return sequence1.Concat(sequence2.SelectMany<IEnumerable<T>, T>());
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> sequence1, params T[] sequence2)
        {
            return sequence1.Concat(sequence2.AsEnumerable());
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> sequence1, params T[] sequence2)
        {
            return sequence1.Except(sequence2.AsEnumerable());
        }

        public static IQueryable<T> Except<T>(this IQueryable<T> sequence1, params T[] sequence2)
        {
            return sequence1.Except(sequence2.AsEnumerable());
        }

        public static bool SequenceEqual<T>(this IEnumerable<T> sequence1, IEnumerable<T> sequence2, bool ignoreOrder)
        {
            if (!ignoreOrder)
            {
                return sequence1.SequenceEqual(sequence2);
            }
            return !sequence1.Except(sequence2).Any() && !sequence2.Except(sequence1).Any();
        }

        public static IEnumerable<string> Split(this string sequence, string separator, StringComparison comparisonType)
        {
            return sequence.Split(separator, comparisonType, StringSplitOptions.None);
        }

        public static IEnumerable<string> Split(this string sequence, string separator, StringComparison comparisonType, StringSplitOptions options)
        {
            var element = default(string);
            for (int a = 0, b = sequence.Length, c = separator.Length, d = 0; a < b; a = d)
            {
                d = sequence.IndexOf(separator, a, comparisonType);
                if (d == -1)
                {
                    element = sequence.Substring(a, b - a);
                    d = b;
                }
                else
                {
                    element = sequence.Substring(a, d - a);
                    d += c;
                }
                if (options.HasFlag(StringSplitOptions.RemoveEmptyEntries) && string.IsNullOrWhiteSpace(element))
                {
                    continue;
                }
                yield return element.Trim();
            }
        }
    }
}
