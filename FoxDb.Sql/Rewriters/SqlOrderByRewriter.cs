using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public abstract class SqlOrderByRewriter : SqlQueryRewriter
    {
        public SqlOrderByRewriter(IDatabase database)
            : base(database)
        {
        }

        protected override void VisitSort(IFragmentBuilder parent, IQueryGraphBuilder graph, ISortBuilder expression)
        {
            throw new NotImplementedException();
        }
    }
}
