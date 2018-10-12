using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SQLiteCreateWriter : SqlCreateWriter
    {
        public SQLiteCreateWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {
        }

        protected override void VisitRelation(ICreateBuilder expression, IRelationBuilder relation)
        {
            //TODO: SQLite cannot ALTER TABLE ADD FOREIGN KEY which makes this kind of thing tricky.
        }
    }
}
