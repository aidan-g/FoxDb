using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SQLiteUpdateWriter : SQLiteQueryWriter
    {
        public SQLiteUpdateWriter(IFragmentBuilder parent, IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(parent, database, visitor, parameterNames)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Update;
            }
        }

        protected override T OnWrite<T>(T fragment)
        {
            if (fragment is IUpdateBuilder)
            {
                var expression = fragment as IUpdateBuilder;
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.UPDATE);
                this.VisitTable(expression.Table);
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.SET);
                this.Visit(expression.Expressions);
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
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.Identifier(expression.Column.ColumnName));
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
