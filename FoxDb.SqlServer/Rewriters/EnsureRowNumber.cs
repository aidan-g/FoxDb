using FoxDb.Interfaces;
using System;
using System.Linq;

namespace FoxDb
{
    public class EnsureRowNumber : SqlSelectRewriter
    {
        public EnsureRowNumber(IDatabase database)
            : base(database)
        {
        }

        protected override void VisitSource(IFragmentBuilder parent, IQueryGraphBuilder graph, ISourceBuilder expression)
        {
            var filter = graph.Fragment<IFilterBuilder>();
            if (!filter.Offset.HasValue)
            {
                return;
            }
            var table = expression.Tables.FirstOrDefault();
            graph.Source.Expressions.Clear();
            graph.Source.AddSubQuery(
                this.Database.QueryFactory.Build().With(subquery =>
                {
                    subquery.Output.AddOperator(QueryOperator.Star);
                    subquery.Output.AddWindowFunction(
                        SqlServerWindowFunction.RowNumber,
                        subquery.Output.CreateSubQuery(
                            this.Database.QueryFactory.Build().With(
                                over => over.Sort.AddColumns(table.Table.PrimaryKeys)
                            )
                        )
                    ).Alias = string.Format(
                        "{0}_RowNumber",
                        filter.Id
                    );
                    subquery.Source.AddTable(table.Table);
                })
            ).Alias = table.Table.TableName;
        }
    }
}
