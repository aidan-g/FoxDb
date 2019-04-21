using FoxDb.Interfaces;
using System.Linq;

namespace FoxDb
{
    public class SqlWhereRewriter : SqlQueryRewriter
    {
        public SqlWhereRewriter(IDatabase database)
            : base(database)
        {
        }

        protected override void VisitFilter(IFragmentBuilder parent, IQueryGraphBuilder graph, IFilterBuilder expression)
        {
            var sequence = Enumerable.Concat<IFragmentBuilder>(
                expression.Expressions.OfType<IColumnBuilder>(),
                expression.Expressions.OfType<IUnaryExpressionBuilder>().Where(unary => unary.Expression is IColumnBuilder)
           ).ToArray();
            foreach (var element in sequence)
            {
                expression.Expressions.Remove(element);
                expression.Expressions.Add(expression.CreateBinary(
                    element,
                    QueryOperator.Equal,
                    element.CreateConstant(1)
                ));
            }
        }
    }
}
