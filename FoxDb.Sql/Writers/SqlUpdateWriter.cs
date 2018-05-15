using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SqlUpdateWriter : SqlQueryWriter
    {
        public SqlUpdateWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters) : base(parent, graph, database, visitor, parameters)
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
                if (expression.Table != null && expression.Expressions.Any())
                {
                    this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.UPDATE);
                    this.VisitTable(expression.Table);
                    this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.SET);
                    this.Visit(expression.Expressions);
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
                    this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.LIST_DELIMITER);
                }
                this.Visit(expression);
            }
        }

        protected override void VisitColumn(IColumnBuilder expression)
        {
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.Identifier(expression.Column.ColumnName));
        }

        public override IFragmentBuilder Clone()
        {
            throw new NotImplementedException();
        }
    }
}
