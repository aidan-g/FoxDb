using FoxDb.Interfaces;
using System;
using System.Linq;

namespace FoxDb
{
    public class EnsureOrderBy : SqlOrderByRewriter
    {
        public EnsureOrderBy(IDatabase database)
            : base(database)
        {

        }

        protected override void VisitSort(IFragmentBuilder parent, IQueryGraphBuilder graph, ISortBuilder expression)
        {
            if (expression.Expressions.Any())
            {
                return;
            }
            var filter = graph.Fragment<IFilterBuilder>();
            if (!filter.Limit.HasValue && !filter.Offset.HasValue)
            {
                return;
            }
            var table = graph.Source.Tables.FirstOrDefault();
            if (table == null)
            {
                throw new NotImplementedException();
            }
            var sort = graph.Parent.Fragment<ISortBuilder>();
            if (sort != null)
            {
                expression.Expressions.AddRange(sort.GetColumns(table.Table));
            }
            if (expression.IsEmpty())
            {
                expression.AddColumns(table.Table.PrimaryKeys);
            }
        }
    }
}
