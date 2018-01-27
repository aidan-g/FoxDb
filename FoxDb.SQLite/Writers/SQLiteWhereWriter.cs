using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SQLiteWhereWriter : SQLiteQueryWriter
    {
        public SQLiteWhereWriter(IFragmentBuilder parent, IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(parent, database, visitor, parameterNames)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Filter;
            }
        }

        protected override T OnWrite<T>(T fragment)
        {
            if (fragment is IFilterBuilder)
            {
                var expression = fragment as IFilterBuilder;
                if (expression.Expressions.Any())
                {
                    this.Builder.AppendFormat("{0} ", SQLiteSyntax.WHERE);
                    this.Visit(expression.Expressions);
                }
                if (expression.Limit != 0)
                {
                    this.Visitor.Visit(this, new LimitBuilder(this, this.Graph, expression.Limit));
                }
                if (expression.Offset != 0)
                {
                    this.Visitor.Visit(this, new OffsetBuilder(this, this.Graph, expression.Offset));
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
                    this.Builder.AppendFormat("{0} ", SQLiteSyntax.AND_ALSO);
                }
                this.Visit(expression);
            }
        }

        protected override void VisitUnary(IUnaryExpressionBuilder expression)
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.OPEN_PARENTHESES);
            base.VisitUnary(expression);
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.CLOSE_PARENTHESES);
        }

        protected override void VisitBinary(IBinaryExpressionBuilder expression)
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.OPEN_PARENTHESES);
            base.VisitBinary(expression);
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.CLOSE_PARENTHESES);
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
