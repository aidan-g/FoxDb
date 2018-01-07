using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoxDb
{
    public class SQLiteWhereWriter : SQLiteQueryWriter
    {
        public SQLiteWhereWriter(IDatabase database, IQueryGraphVisitor visitor, StringBuilder builder, ICollection<string> parameterNames) : base(database, visitor, builder, parameterNames)
        {

        }

        public override void Write(IFragmentBuilder fragment)
        {
            if (fragment is IWhereBuilder)
            {
                var expression = fragment as IWhereBuilder;
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.WHERE);
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
                    this.Builder.AppendFormat("{0} ", SQLiteSyntax.AND_ALSO);
                }
                this.Visit(expression);
            }
        }
    }
}
