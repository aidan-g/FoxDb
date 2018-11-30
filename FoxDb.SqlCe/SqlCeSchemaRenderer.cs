using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlCeSchemaRenderer : SqlSchemaRenderer
    {
        public SqlCeSchemaRenderer(IDatabase database)
            : base(database)
        {
        }

        protected override SqlQueryFragment CreateQueryFragment(IFragmentTarget target)
        {
            return new SqlCeQueryFragment(target);
        }

        protected override void VisitCreate(IFragmentBuilder parent, IQueryGraphBuilder graph, ICreateBuilder expression)
        {
            this.Push(new SqlCeCreateWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }
    }
}
