using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlServerQueryRenderer : SqlQueryRenderer
    {
        public SqlServerQueryRenderer(IDatabase database)
            : base(database)
        {
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
    }
}
