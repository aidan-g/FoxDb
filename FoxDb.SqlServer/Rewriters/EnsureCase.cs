using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class EnsureCase : SqlSelectRewriter
    {
        public static Func<IFragmentBuilder, IQueryGraphBuilder, IOutputBuilder, bool> Predicate = (parent, graph, expression) =>
        {
            return GetExpressions(expression).Any();
        };

        public static IEnumerable<IExpressionBuilder> GetExpressions(IFragmentContainer container)
        {
            return container.GetExpressions<IFunctionBuilder>(function => function.Function == QueryFunction.Exists);
        }

        public EnsureCase(IDatabase database) : base(database)
        {
        }

        protected override void VisitOutput(IFragmentBuilder parent, IQueryGraphBuilder graph, IOutputBuilder expression)
        {
            var sequence = GetExpressions(expression).ToArray();
            foreach (var element in sequence)
            {
                expression.Expressions.Remove(element);
                expression.AddCase(
                    expression.CreateCaseCondition(
                        element, expression.CreateConstant(1)
                    ),
                    expression.CreateCaseCondition(expression.CreateConstant(0))
                ).Alias = element.Alias;
            }
        }
    }
}
