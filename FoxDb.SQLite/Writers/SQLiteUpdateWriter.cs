using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SQLiteUpdateWriter : SQLiteQueryWriter
    {
        public SQLiteUpdateWriter(IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(database, visitor, parameterNames)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Update;
            }
        }

        public override void Write(IFragmentBuilder fragment)
        {
            if (fragment is IUpdateBuilder)
            {
                var expression = fragment as IUpdateBuilder;
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.UPDATE);
                this.VisitTable(expression.Table);
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.SET);
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
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.Identifier(expression.Column.ColumnName));
        }
    }
}
