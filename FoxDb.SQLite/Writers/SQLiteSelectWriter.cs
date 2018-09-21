using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SQLiteSelectWriter : SqlSelectWriter
    {
        public SQLiteSelectWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {
        }

        protected override IDictionary<QueryFunction, SqlQueryWriterDialectHandler> GetFunctions()
        {
            var functions = base.GetFunctions();
            functions[SQLiteQueryFunction.LastInsertRowId] = writer => (writer.Database.QueryFactory.Dialect as SQLiteQueryDialect).LAST_INSERT_ROWID;
            return functions;
        }
    }
}
