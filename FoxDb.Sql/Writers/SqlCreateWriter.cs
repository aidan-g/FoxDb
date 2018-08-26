using FoxDb.Interfaces;
using System;
using System.Linq;
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
                var columns = expression.Expressions.OfType<IColumnBuilder>();
                var indexes = expression.Expressions.OfType<IIndexBuilder>();
                if (columns.Any())
                {
                    this.VisitTable(expression, columns);
                }
                if (indexes.Any())
                {
                    foreach (var index in indexes)
                    {
                        this.VisitIndex(expression, index);
                    }
                }
                return fragment;
            }
            throw new NotImplementedException();
        }

        protected virtual void VisitTable(ICreateBuilder expression, IEnumerable<IColumnBuilder> columns)
        {
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CREATE);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.TABLE);
            this.Visit(expression.Table);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.OPEN_PARENTHESES);
            this.Visit(columns);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CLOSE_PARENTHESES);
        }


        protected virtual void VisitIndex(ICreateBuilder expression, IIndexBuilder index)
        {
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.BATCH);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CREATE);
            if (index.Index.Flags.HasFlag(IndexFlags.Unique))
            {
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.UNIQUE);
            }
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.INDEX);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.Identifier(index.Index.IndexName));
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.ON);
            this.Visit(expression.Table);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.OPEN_PARENTHESES);
            this.Visit(index.Columns);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CLOSE_PARENTHESES);
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
            if (expression.Column.ColumnType.IsNullable)
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
