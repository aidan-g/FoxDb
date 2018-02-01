using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SqlWhereWriter : SqlQueryWriter
    {
        public SqlWhereWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters) : base(parent, graph, database, visitor, parameters)
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

        protected override void VisitUnary(IUnaryExpressionBuilder expression)
        {
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.OPEN_PARENTHESES);
            base.VisitUnary(expression);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CLOSE_PARENTHESES);
        }

        protected override void VisitBinary(IBinaryExpressionBuilder expression)
        {
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.OPEN_PARENTHESES);
            base.VisitBinary(expression);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CLOSE_PARENTHESES);
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
