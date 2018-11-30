using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlServerSchemaRenderer : SqlSchemaRenderer
    {
        public SqlServerSchemaRenderer(IDatabase database)
            : base(database)
        {
        }

        protected override SqlQueryFragment CreateQueryFragment(IFragmentTarget target)
        {
            return new SqlServerQueryFragment(target);
        }

        protected override void VisitCreate(IFragmentBuilder parent, IQueryGraphBuilder graph, ICreateBuilder expression)
        {
            this.Push(new SqlServerCreateWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }
    }
}
