using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlCeFromWriter : SqlFromWriter
    {
        public SqlCeFromWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters) : base(parent, graph, database, visitor, parameters)
        {
        }
    }
}
