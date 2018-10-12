using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

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
                var tables = expression.Expressions.OfType<ITableBuilder>();
                var columns = expression.Expressions.OfType<IColumnBuilder>();
                var relations = expression.Expressions.OfType<IRelationBuilder>();
                var indexes = expression.Expressions.OfType<IIndexBuilder>();
                var batches = new List<Action>();
                foreach (var table in tables)
                {
                    batches.Add(() => this.VisitTable(
                        expression,
                        table,
                        columns.Where(
                            column => TableComparer.TableConfig.Equals(column.Column.Table, table.Table)
                        )
                    ));
                }
                foreach (var relation in relations)
                {
                    batches.Add(() => this.VisitRelation(expression, relation));
                }
                foreach (var index in indexes)
                {
                    batches.Add(() => this.VisitIndex(expression, index));
                }
                this.VisitBatches(batches);
                return fragment;
            }
            throw new NotImplementedException();
        }

        protected virtual void VisitTable(ICreateBuilder expression, ITableBuilder table, IEnumerable<IColumnBuilder> columns)
        {
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CREATE);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.TABLE);
            this.Visit(table);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.OPEN_PARENTHESES);
            this.Visit(columns);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CLOSE_PARENTHESES);
        }

        protected virtual void VisitRelation(ICreateBuilder expression, IRelationBuilder relation)
        {
            var map = relation.Relation.Expression.GetColumnMap();
            switch (relation.Relation.Flags.GetMultiplicity())
            {
                case RelationFlags.OneToOne:
                case RelationFlags.OneToMany:
                    {
                        var leftColumn = map[relation.Relation.LeftTable].SingleOrDefault();
                        var rightColumn = map[relation.Relation.RightTable].SingleOrDefault();
                        this.VisitRelation(expression, relation, relation.Relation.LeftTable, relation.Relation.RightTable, leftColumn, rightColumn);
                    }
                    break;
                case RelationFlags.ManyToMany:
                    {
                        var leftColumn = map[relation.Relation.LeftTable].SingleOrDefault();
                        var rightColumn = relation.Relation.Expression.GetOppositeExpression<IColumnBuilder, IColumnBuilder>(
                            column => object.ReferenceEquals(column.Column, leftColumn)
                        ).Column;
                        this.VisitRelation(expression, relation, relation.Relation.LeftTable, relation.Relation.MappingTable, leftColumn, rightColumn);
                    }
                    this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.BATCH);
                    {
                        var leftColumn = map[relation.Relation.RightTable].SingleOrDefault();
                        var rightColumn = relation.Relation.Expression.GetOppositeExpression<IColumnBuilder, IColumnBuilder>(
                            column => object.ReferenceEquals(column.Column, leftColumn)
                        ).Column;
                        this.VisitRelation(expression, relation, relation.Relation.RightTable, relation.Relation.MappingTable, leftColumn, rightColumn);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual void VisitRelation(ICreateBuilder expression, IRelationBuilder relation, ITableConfig leftTable, ITableConfig rightTable, IColumnConfig leftColumn, IColumnConfig rightColumn)
        {
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.ALTER);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.TABLE);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.Identifier(rightTable.TableName));
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.ADD);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CONSTRAINT);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.Identifier(Conventions.RelationName(leftTable, rightTable)));
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.FOREIGN_KEY);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.OPEN_PARENTHESES);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.Identifier(rightColumn.ColumnName));
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CLOSE_PARENTHESES);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.REFERENCES);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.Identifier(leftTable.TableName));
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.OPEN_PARENTHESES);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.Identifier(leftColumn.ColumnName));
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CLOSE_PARENTHESES);
        }

        protected virtual void VisitIndex(ICreateBuilder expression, IIndexBuilder index)
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
            if (index.Index.Expression != null && !index.Index.Expression.IsEmpty())
            {
                this.Visitor.Visit(
                    this,
                    this.Graph,
                    this.Fragment<IFilterBuilder>().With(
                        filter => filter.Expressions.Add(index.Index.Expression)
                    )
                );
            }
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
