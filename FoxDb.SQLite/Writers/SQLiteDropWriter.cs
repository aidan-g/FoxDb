using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SQLiteDropWriter : SqlDropWriter
    {
        public SQLiteDropWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {
        }

        protected override void VisitRelation(IDropBuilder expression, IRelationBuilder relation)
        {
            //TODO: SQLite cannot ALTER TABLE DROP FOREIGN KEY which makes this kind of thing tricky.
        }
    }
}
