using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlServerQueryRenderer : SqlQueryRenderer
    {
        public SqlServerQueryRenderer(IDatabase database)
            : base(database)
        {
        }

        protected override IDictionary<FragmentType, QueryGraphVisitorHandler> GetHandlers()
        {
            var handlers = base.GetHandlers();
            handlers[SqlServerQueryFragment.TableHint] = (visitor, parent, graph, fragment) => (visitor as SqlServerQueryRenderer).VisitTableHint(parent, graph, fragment as ITableHintBuilder);
            return handlers;
        }

        protected override SqlQueryFragment CreateQueryFragment(IFragmentTarget target)
        {
            return new SqlServerQueryFragment(target);
        }

        protected override void VisitOutput(IFragmentBuilder parent, IQueryGraphBuilder graph, IOutputBuilder expression)
        {
            this.Push(new SqlServerSelectWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitSource(IFragmentBuilder parent, IQueryGraphBuilder graph, ISourceBuilder expression)
        {
            this.Push(new SqlServerFromWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected virtual void VisitTableHint(IFragmentBuilder parent, IQueryGraphBuilder graph, ITableHintBuilder expression)
        {
            this.Push(new SqlServerTableHintWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitCreate(IFragmentBuilder parent, IQueryGraphBuilder graph, ICreateBuilder expression)
        {
            this.Push(new SqlServerCreateWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }
    }
}
