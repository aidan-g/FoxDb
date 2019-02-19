using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SQLiteSelectWriter : SqlSelectWriter
    {
        protected override IDictionary<QueryWindowFunction, SqlQueryWriterVisitorHandler> GetWindowFunctions()
        {
            var functions = base.GetWindowFunctions();
            functions[SQLiteWindowFunction.RowNumber] = (writer, fragment) => (writer as SQLiteSelectWriter).VisitRowNumber(fragment as IWindowFunctionBuilder);
            return functions;
        }

        public SQLiteSelectWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {
            this.Dialect = this.Database.QueryFactory.Dialect as SQLiteQueryDialect;
        }

        public SQLiteQueryDialect Dialect { get; private set; }

        protected override IDictionary<QueryFunction, SqlQueryWriterVisitorHandler> GetFunctions()
        {
            var functions = base.GetFunctions();
            functions[SQLiteQueryFunction.LastInsertRowId] = (writer, fragment) => this.Builder.AppendFormat(
                "{0} ",
                (writer.Database.QueryFactory.Dialect as SQLiteQueryDialect).LAST_INSERT_ID
            );
            return functions;
        }

        protected virtual void VisitRowNumber(IWindowFunctionBuilder expression)
        {
            this.Builder.AppendFormat(
                "{0}{1}{2} ",
                this.Dialect.ROW_NUMBER,
                this.Dialect.OPEN_PARENTHESES,
                this.Dialect.CLOSE_PARENTHESES
            );
            this.Builder.AppendFormat(
                "{0}{1} ",
                this.Dialect.OVER,
                this.Dialect.OPEN_PARENTHESES
            );
            if (expression.Expressions.Any())
            {
                this.AddRenderContext(RenderHints.FunctionArgument);
                try
                {
                    this.Visit(expression.Expressions);
                }
                finally
                {
                    this.RemoveRenderContext();
                }
            }
            this.Builder.AppendFormat("{0} ", this.Dialect.CLOSE_PARENTHESES);
        }
    }
}
