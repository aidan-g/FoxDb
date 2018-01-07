using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoxDb
{
    public class SQLiteInsertWriter : SQLiteQueryWriter
    {
        public SQLiteInsertWriter(IDatabase database, IQueryGraphVisitor visitor, StringBuilder builder, ICollection<string> parameterNames) : base(database, visitor, builder, parameterNames)
        {

        }

        public override void Write(IFragmentBuilder fragment)
        {
            if (fragment is IInsertBuilder)
            {
                var expression = fragment as IInsertBuilder;
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.INSERT);
                this.Visit(expression.Table);
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.OPEN_PARENTHESES);
                this.Visit(expression.Expressions);
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.CLOSE_PARENTHESES);
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
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.Identifier(expression.Column.ColumnName));
        }
    }
}
