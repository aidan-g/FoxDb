using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlCreateWriter : SqlQueryWriter
    {
        public SqlCreateWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Create;
            }
        }

        protected override T OnWrite<T>(T fragment)
        {
            if (fragment is ICreateBuilder)
            {
                var expression = fragment as ICreateBuilder;
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CREATE_TABLE);
                this.Visit(expression.Table);
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.OPEN_PARENTHESES);
                this.Visit(expression.Expressions);
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CLOSE_PARENTHESES);
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
            this.VisitType(expression.Column.ColumnType);
            if (expression.Column.IsPrimaryKey)
            {
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.PRIMARY_KEY);
            }
            if (expression.Column.IsNullable)
            {
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.NULL);
            }
            else
            {
                this.Builder.AppendFormat("{0} {1} ", this.Database.QueryFactory.Dialect.NOT, this.Database.QueryFactory.Dialect.NULL);
            }
        }

        public override IFragmentBuilder Clone()
        {
            throw new NotImplementedException();
        }
    }
}
