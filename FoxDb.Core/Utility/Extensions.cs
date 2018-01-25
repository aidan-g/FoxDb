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

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> sequence1, params T[] sequence2)
        {
            return sequence1.Concat(sequence2.AsEnumerable());
        }
    }
}
