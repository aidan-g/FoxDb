using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SQLiteSelectWriter : SqlSelectWriter
    {
        static SQLiteSelectWriter()
        {
            Functions[SQLiteQueryFunction.LastInsertRowId] = writer => (writer.Database.QueryFactory.Dialect as SQLiteQueryDialect).LAST_INSERT_ROWID;
        }

        public SQLiteSelectWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters) : base(parent, graph, database, visitor, parameters)
        {
        }
    }
}
