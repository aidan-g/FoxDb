using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlServerFromWriter : SqlFromWriter
    {
        public SqlServerFromWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters) : base(parent, graph, database, visitor, parameters)
        {

        }

        protected override void VisitTable(ITableBuilder expression)
        {
            base.VisitTable(expression);
            if (expression.LockHints != LockHints.None)
            {
                this.AddRenderContext(RenderHints.AssociativeExpression);
                try
                {
                    this.Visitor.Visit(this, this.Graph, new TableHintBuilder(this, this.Graph, expression.LockHints));
                }
                finally
                {
                    this.RemoveRenderContext();
                }
            }
        }
    }
}
