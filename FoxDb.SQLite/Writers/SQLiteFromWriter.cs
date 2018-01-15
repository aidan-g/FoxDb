using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoxDb
{
    public class SQLiteFromWriter : SQLiteQueryWriter
    {
        public SQLiteFromWriter(IDatabase database, IQueryGraphVisitor visitor, StringBuilder builder, ICollection<string> parameterNames) : base(database, visitor, builder, parameterNames)
        {

        }

        public override void Write(IFragmentBuilder fragment)
        {
            if (fragment is IFromBuilder)
            {
                var expression = fragment as IFromBuilder;
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.FROM);
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
                    if (expression is ITableBuilder || expression is ISubQueryBuilder)
                    {
                        this.Builder.AppendFormat("{0} ", SQLiteSyntax.LIST_DELIMITER);
                    }
                    else if (expression is IRelationBuilder)
                    {
                        //Nothing to do.
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                this.Visit(expression);
            }
        }

        protected override void VisitSubQuery(ISubQueryBuilder expression)
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.OPEN_PARENTHESES);
            base.VisitSubQuery(expression);
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.CLOSE_PARENTHESES);
        }
    }
}
