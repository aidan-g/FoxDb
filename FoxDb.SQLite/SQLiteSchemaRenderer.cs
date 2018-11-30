using FoxDb.Interfaces;

namespace FoxDb
{
    public class SQLiteSchemaRenderer : SqlSchemaRenderer
    {
        public SQLiteSchemaRenderer(IDatabase database)
            : base(database)
        {

        }

        protected override SqlQueryFragment CreateQueryFragment(IFragmentTarget target)
        {
            return new SQLiteQueryFragment(target);
        }

        protected override void VisitCreate(IFragmentBuilder parent, IQueryGraphBuilder graph, ICreateBuilder expression)
        {
            this.Push(new SQLiteCreateWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitDrop(IFragmentBuilder parent, IQueryGraphBuilder graph, IDropBuilder expression)
        {
            this.Push(new SQLiteDropWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }
    }
}
