using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public abstract class SqlSelectRewriter : SqlQueryRewriter
    {
        public SqlSelectRewriter(IDatabase database)
            : base(database)
        {
        }

        protected override void VisitOutput(IFragmentBuilder parent, IQueryGraphBuilder graph, IOutputBuilder expression)
        {
            throw new NotImplementedException();
        }
    }
}
