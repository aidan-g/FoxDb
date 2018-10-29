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

        protected override IDictionary<QueryFunction, SqlQueryWriterVisitorHandler> GetFunctions()
        {
            var functions = base.GetFunctions();
            functions[SQLiteQueryFunction.LastInsertRowId] = (writer, fragment) => this.Builder.AppendFormat(
                "{0} ",
                (writer.Database.QueryFactory.Dialect as SQLiteQueryDialect).LAST_INSERT_ID
            );
            return functions;
        }
    }
}
