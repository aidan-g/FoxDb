using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SQLiteSelectWriter : SQLiteQueryWriter
    {
        public SQLiteSelectWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(parent, graph, database, visitor, parameterNames)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Output;
            }
        }

        protected override T OnWrite<T>(T fragment)
        {
            if (fragment is IOutputBuilder)
            {
                var expression = fragment as IOutputBuilder;
                if (expression.Expressions.Any())
                {
                    this.Builder.AppendFormat("{0} ", SQLiteSyntax.SELECT);
                    this.Visit(expression.Expressions);
                }
                return fragment;
            }
            throw new NotImplementedException();
        }

        protected override void Visit(IEnumerable<IFragmentBuilder> expressions)
        {
            var first = true;
            foreach (var expression in expressions)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    this.Builder.AppendFormat("{0} ", SQLiteSyntax.LIST_DELIMITER);
                }
                this.Visit(expression);
            }
        }

        protected override void VisitColumn(IColumnBuilder expression)
        {
            base.VisitColumn(expression);
            this.VisitAlias(expression.Alias);
        }

        protected override void VisitParameter(IParameterBuilder expression)
        {
            base.VisitParameter(expression);
            this.VisitAlias(expression.Alias);
        }

        protected override void VisitFunction(IFunctionBuilder expression)
        {
            base.VisitFunction(expression);
            this.VisitAlias(expression.Alias);
        }

        protected override void VisitSubQuery(ISubQueryBuilder expression)
        {
            if (this.GetRenderContext().HasFlag(RenderHints.FunctionArgument))
            {
                base.VisitSubQuery(expression);
            }
            else
            {
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.OPEN_PARENTHESES);
                base.VisitSubQuery(expression);
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.CLOSE_PARENTHESES);
                this.VisitAlias(expression.Alias);
            }
        }

        public override IFragmentBuilder Clone()
        {
            throw new NotImplementedException();
        }

        public override string DebugView
        {
            get
            {
                return string.Format("{{}}");
            }
        }
    }
}
