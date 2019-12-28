using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public abstract class SqlWhereRewriter : SqlQueryRewriter
    {
        public SqlWhereRewriter(IDatabase database)
            : base(database)
        {
        }

        protected override void VisitFilter(IFragmentBuilder parent, IQueryGraphBuilder graph, IFilterBuilder expression)
        {
            throw new NotImplementedException();
        }
    }
}
