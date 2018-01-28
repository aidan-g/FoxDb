using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SQLiteInsertWriter : SQLiteQueryWriter
    {
        public SQLiteInsertWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(parent, graph, database, visitor, parameterNames)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Add;
            }
        }

        protected override T OnWrite<T>(T fragment)
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
