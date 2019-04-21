using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class FilterRowNumber : SqlWhereRewriter
    {
        public FilterRowNumber(IDatabase database)
            : base(database)
        {
        }

        protected override void VisitFilter(IFragmentBuilder parent, IQueryGraphBuilder graph, IFilterBuilder expression)
        {
            if (!expression.Offset.HasValue)
            {
                return;
            }
            var offset = expression.Offset.Value + 1;
            expression.Add().With(filter =>
            {
                filter.Left = filter.CreateIdentifier(
                    string.Format(
                        "{0}_RowNumber",
                        expression.Id
                    )
                );
                if (expression.Limit.HasValue)
                {
                    var limit = expression.Limit.Value - 1;
                    filter.Operator = filter.CreateOperator(QueryOperator.Between);
                    filter.Right = filter.CreateBinary(
                        filter.CreateConstant(offset),
                        filter.CreateOperator(QueryOperator.AndAlso),
                        filter.CreateConstant(offset + limit)
                    );
                }
                else
                {
                    filter.Operator = filter.CreateOperator(QueryOperator.GreaterOrEqual);
                    filter.Right = filter.CreateConstant(offset);
                }
            });
        }
    }
}
