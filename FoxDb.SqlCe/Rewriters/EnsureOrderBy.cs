using FoxDb.Interfaces;
using System;
using System.Linq;

namespace FoxDb
{
    public class EnsureOrderBy : SqlOrderByRewriter
    {
        public static Func<IFragmentBuilder, IQueryGraphBuilder, ISortBuilder, bool> Predicate = (parent, graph, expression) =>
        {
            if (expression.Expressions.Any())
            {
                return false;
            }
            var filter = graph.Fragment<IFilterBuilder>();
            return filter.Offset.HasValue;
        };

        public EnsureOrderBy(IDatabase database)
            : base(database)
        {

        }

        protected override void VisitSort(IFragmentBuilder parent, IQueryGraphBuilder graph, ISortBuilder expression)
        {
            var table = graph.Source.Tables.FirstOrDefault();
            if (table == null)
            {
                throw new NotImplementedException();
            }
            expression.AddColumns(table.Table.PrimaryKeys);
            base.VisitSort(parent, graph, expression);
        }
    }
}
