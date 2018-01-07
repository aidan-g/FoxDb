using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoxDb
{
    public class SQLiteSelectWriter : SQLiteQueryWriter
    {
        public SQLiteSelectWriter(IDatabase database, IQueryGraphVisitor visitor, StringBuilder builder, ICollection<string> parameterNames) : base(database, visitor, builder, parameterNames)
        {

        }

        public override void Write(IFragmentBuilder fragment)
        {
            if (fragment is ISelectBuilder)
            {
                var expression = fragment as ISelectBuilder;
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.SELECT);
                this.Visit(expression.Expressions);
                return;
            }
            throw new NotImplementedException();
        }

        protected override void Visit(IEnumerable<IExpressionBuilder> expressions)
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
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.OPEN_PARENTHESES);
            base.VisitSubQuery(expression);
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.CLOSE_PARENTHESES);
            this.VisitAlias(expression.Alias);
        }

        protected virtual void VisitAlias(string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                return;
            }
            this.Builder.AppendFormat("{0} {1} ", SQLiteSyntax.AS, SQLiteSyntax.Identifier(alias));
        }
    }
}
