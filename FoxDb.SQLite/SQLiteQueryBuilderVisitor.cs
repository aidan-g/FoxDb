using FoxDb.Interfaces;

namespace FoxDb
{
    public class SQLiteQueryBuilderVisitor : SqlQueryBuilderVisitor
    {
        static SQLiteQueryBuilderVisitor()
        {
            Handlers[SQLiteQueryFragment.Limit] = (visitor, parent, graph, fragment) => (visitor as SQLiteQueryBuilderVisitor).VisitLimit(parent, graph, fragment as ILimitBuilder);
            Handlers[SQLiteQueryFragment.Offset] = (visitor, parent, graph, fragment) => (visitor as SQLiteQueryBuilderVisitor).VisitOffset(parent, graph, fragment as IOffsetBuilder);
        }


        public SQLiteQueryBuilderVisitor(IDatabase database) : base(database)
        {

        }

        protected override SqlQueryFragment CreateQueryFragment(IFragmentTarget target)
        {
            return new SQLiteQueryFragment(target);
        }

        protected override void VisitFilter(IFragmentBuilder parent, IQueryGraphBuilder graph, IFilterBuilder expression)
        {
            this.Push(new SQLiteWhereWriter(parent, graph, this.Database, this, this.ParameterNames));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected virtual void VisitLimit(IFragmentBuilder parent, IQueryGraphBuilder graph, ILimitBuilder expression)
        {
            this.Push(new SQLiteLimitWriter(parent, graph, this.Database, this, this.ParameterNames));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected virtual void VisitOffset(IFragmentBuilder parent, IQueryGraphBuilder graph, IOffsetBuilder expression)
        {
            this.Push(new SQLiteOffsetWriter(parent, graph, this.Database, this, this.ParameterNames));
            this.Peek.Write(expression);
            this.Pop();
        }
    }
}
