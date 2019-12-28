using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SqlWhereWriter : SqlQueryWriter
    {
        public SqlWhereWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Filter;
            }
        }

        protected override T OnWrite<T>(T fragment)
        {
            if (fragment is IFilterBuilder)
            {
                var expression = fragment as IFilterBuilder;
                if (this.Graph.RelationManager.HasExternalRelations || expression.Expressions.Any())
                {
                    var first = true;
                    this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.WHERE);
                    if (expression.Expressions.Any())
                    {
                        this.Visit(expression.Expressions);
                        first = false;
                    }
                    this.Visit(this.Graph.RelationManager.Calculator, this.Graph.RelationManager.CalculatedRelations, first);
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
                    this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.AND_ALSO);
                }
                this.Visit(expression);
            }
        }

        protected virtual void Visit(IEntityRelationCalculator calculator, IEnumerable<IEntityRelation> relations, bool first)
        {
            foreach (var relation in relations)
            {
                var expression = default(IBinaryExpressionBuilder);
                if (relation.IsExternal)
                {
                    expression = calculator.Extern(relation);
                }
                else
                {
                    continue;
                }
                if (first)
                {
                    first = false;
                }
                else
                {
                    this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.AND_ALSO);
                }
                this.Visit(expression);
            }
        }

        protected override void VisitTable(ITableBuilder expression)
        {
            this.Visit(expression.CreateColumn(expression.Table.PrimaryKey));
        }

        protected override void VisitSubQuery(ISubQueryBuilder expression)
        {
            if (this.GetRenderContext().HasFlag(RenderHints.FunctionArgument))
            {
                base.VisitSubQuery(expression);
            }
            else
            {
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.OPEN_PARENTHESES);
                base.VisitSubQuery(expression);
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CLOSE_PARENTHESES);
            }
        }

        protected override void VisitSequence(ISequenceBuilder expression)
        {
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.OPEN_PARENTHESES);
            base.VisitSequence(expression);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CLOSE_PARENTHESES);
        }

        protected override void VisitBinary(IBinaryExpressionBuilder expression)
        {
            var column = expression.GetExpression<IColumnBuilder>();
            var parameter = expression.GetExpression<IParameterBuilder>();
            //TODO: This should be done in the re-writer phase but don't have anything to emit the parentheses with.
            //column = @param for a nullable type. We need to create the appropriate filter as null cannot be directly compared.
            if (column != null && column.Column.ColumnType.IsNullable && expression.Operator != null && expression.Operator.Operator == QueryOperator.Equal && parameter != null)
            {
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.OPEN_PARENTHESES);
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.OPEN_PARENTHESES);
                this.Visit(parameter);
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.IS);
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.NULL);
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.AND_ALSO);
                this.Visit(column);
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.IS);
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.NULL);
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CLOSE_PARENTHESES);
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.OR_ELSE);
                base.VisitBinary(expression);
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CLOSE_PARENTHESES);
            }
            else
            {
                base.VisitBinary(expression);
            }
        }

        protected override void VisitColumn(IColumnBuilder expression)
        {
            var parent = this.FragmentContext.Skip(1).FirstOrDefault();
            if (parent != null)
            {
                var enforceBinary = false;
                switch (parent.FragmentType)
                {
                    case FragmentType.Unary:
                        enforceBinary = true;
                        break;
                    case FragmentType.Binary:
                        var binary = parent as IBinaryExpressionBuilder;
                        if (binary.Operator != null)
                        {
                            switch (binary.Operator.Operator)
                            {
                                case QueryOperator.AndAlso:
                                case QueryOperator.OrElse:
                                    enforceBinary = true;
                                    break;
                            }
                        }
                        break;
                    case FragmentType.Filter:
                        enforceBinary = true;
                        break;
                }
                if (enforceBinary)
                {
                    this.Visit(
                        this.CreateBinary(
                            expression,
                            this.CreateOperator(QueryOperator.Equal),
                            this.CreateConstant(1)
                        )
                    );
                    return;
                }
            }
            base.VisitColumn(expression);
        }

        public override IFragmentBuilder Clone()
        {
            throw new NotImplementedException();
        }
    }
}
