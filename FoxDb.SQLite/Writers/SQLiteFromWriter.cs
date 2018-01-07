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

        protected override void VisitSubQuery(ISubQueryBuilder expression)
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.OPEN_PARENTHESES);
            base.VisitSubQuery(expression);
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.CLOSE_PARENTHESES);
        }
    }
}
