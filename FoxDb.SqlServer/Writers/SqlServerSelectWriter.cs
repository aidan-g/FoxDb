using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SqlServerSelectWriter : SqlSelectWriter
    {
        protected override IDictionary<QueryWindowFunction, SqlQueryWriterVisitorHandler> GetWindowFunctions()
        {
            var functions = base.GetWindowFunctions();
            functions[SqlServerWindowFunction.RowNumber] = (writer, fragment) => (writer as SqlServerSelectWriter).VisitRowNumber(fragment as IWindowFunctionBuilder);
            return functions;
        }

        public SqlServerSelectWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {
            this.Dialect = this.Database.QueryFactory.Dialect as SqlServerQueryDialect;
        }

        public SqlServerQueryDialect Dialect { get; private set; }

        protected override T OnWrite<T>(T fragment)
        {
            if (fragment is IOutputBuilder)
            {
                var expression = fragment as IOutputBuilder;
                if (expression.Expressions.Any())
                {
                    this.Builder.AppendFormat("{0} ", this.Dialect.SELECT);
                    if (this.Graph.Filter.Limit.HasValue && !this.Graph.Filter.Offset.HasValue)
                    {
                        this.Builder.AppendFormat("{0} ", this.Dialect.TOP);
                        this.Builder.AppendFormat("{0} ", this.Graph.Filter.Limit);
                        switch (this.Graph.Filter.LimitType)
                        {
                            case LimitType.Percent:
                                this.Builder.AppendFormat("{0} ", this.Dialect.PERCENT);
                                break;
                        }
                    }
                    this.Visit(expression.Expressions);
                    return fragment;
                }
            }
            throw new NotImplementedException();
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
