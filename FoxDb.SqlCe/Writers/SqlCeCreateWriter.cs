using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlCeCreateWriter : SqlCreateWriter
    {
        public SqlCeCreateWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {
            this.Dialect = this.Database.QueryFactory.Dialect as SqlCeQueryDialect;
        }

        public SqlCeQueryDialect Dialect { get; private set; }

        protected override void VisitColumn(IColumnBuilder expression)
        {
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.Identifier(expression.Column.ColumnName));
            this.VisitType(expression.Column.ColumnType);
            if (expression.Column.Flags.HasFlag(ColumnFlags.Generated) && expression.Column.ColumnType.IsNumeric)
            {
                this.Builder.AppendFormat("{0} ", this.Dialect.IDENTITY);
            }
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

        protected override void VisitIndex(ICreateBuilder expression, IIndexBuilder index)
        {
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CREATE);
            if (index.Index.Flags.HasFlag(IndexFlags.Unique))
            {
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.UNIQUE);
            }
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.INDEX);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.Identifier(Conventions.IndexName(index.Index)));
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.ON);
            this.Visit(index.Table);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.OPEN_PARENTHESES);
            this.Visit(index.Columns);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CLOSE_PARENTHESES);
        }
    }
}
