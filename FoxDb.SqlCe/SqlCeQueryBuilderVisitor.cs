using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlCeQueryBuilderVisitor : SqlQueryBuilderVisitor
    {
        public SqlCeQueryBuilderVisitor(IDatabase database) : base(database)
        {
        }

        protected override SqlQueryFragment CreateQueryFragment(IFragmentTarget target)
        {
            return new SqlCeQueryFragment(target);
        }

        protected override void VisitOutput(IFragmentBuilder parent, IQueryGraphBuilder graph, IOutputBuilder expression)
        {
            this.Push(new SqlCeSelectWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }
    }
}
