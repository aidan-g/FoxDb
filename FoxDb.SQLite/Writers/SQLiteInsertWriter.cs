using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SQLiteInsertWriter : SQLiteQueryWriter
    {
        public SQLiteInsertWriter(IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(database, visitor, parameterNames)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Add;
            }
        }

        public override void Write(IFragmentBuilder fragment)
        {
            if (fragment is IAddBuilder)
            {
                var expression = fragment as IAddBuilder;
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.INSERT);
                this.Visit(expression.Table);
                if (!expression.Expressions.Any())
                {
                    this.Builder.AppendFormat("{0} {1} ", SQLiteSyntax.DEFAULT, SQLiteSyntax.VALUES);
                }
                else
                {
                    this.Builder.AppendFormat("{0} ", SQLiteSyntax.OPEN_PARENTHESES);
                    this.Visit(expression.Expressions);
                    this.Builder.AppendFormat("{0} ", SQLiteSyntax.CLOSE_PARENTHESES);
                }
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
