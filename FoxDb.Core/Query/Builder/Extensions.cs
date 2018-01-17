using FoxDb.Interfaces;
using System;
using System.Linq;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static bool IsEmpty(this IFragmentContainer container)
        {
            return !container.Expressions.Any();
        }

        public static T GetExpression<T>(this IFragmentContainer container, Func<T, bool> predicate) where T : IExpressionBuilder
        {
            return container.Expressions.OfType<T>().FirstOrDefault(predicate);
        }
    }
}
