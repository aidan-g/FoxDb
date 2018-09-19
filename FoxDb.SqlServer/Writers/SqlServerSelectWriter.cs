using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SqlServerSelectWriter : SqlSelectWriter
    {
        public SqlServerSelectWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {
            this.Dialect = this.Database.QueryFactory.Dialect as SqlServerQueryDialect;
        }

        public SqlServerQueryDialect Dialect { get; private set; }

        protected override T OnWrite<T>(T fragment)
        {
            if (fragment is IOutputBuilder)
            {
                var expression = fragment as IOutputBuilder;
                if (expression.Expressions.Any())
                {
                    this.Builder.AppendFormat("{0} ", this.Dialect.SELECT);
                    if (this.Graph.Filter.Limit != 0)
                    {
                        this.Builder.AppendFormat("{0} ", this.Dialect.TOP);
                        this.Builder.AppendFormat("{0} ", this.Graph.Filter.Limit);
                        switch (this.Graph.Filter.LimitType)
                        {
                            case LimitType.None:
                                break;
                            case LimitType.Percent:
                                this.Builder.AppendFormat("{0} ", this.Dialect.PERCENT);
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    this.Visit(expression.Expressions);
                }
                return fragment;
            }
            throw new NotImplementedException();
        }

        protected override void VisitUnary(IUnaryExpressionBuilder expression)
        {
            switch (expression.Operator.Operator)
            {
                case QueryOperator.Not:
                    //TODO Assuming expression to negate returns 1 for "true".
                    this.Builder.AppendFormat("{0} ", this.Dialect.CASE);
                    this.Builder.AppendFormat("{0} ", this.Dialect.WHEN);
                    this.Builder.AppendFormat("{0} ", this.Dialect.NOT);
                    this.Visit(expression.Expression);
                    this.Builder.AppendFormat("{0} ", this.Dialect.EQUAL);
                    this.Visit(this.CreateConstant(1));
                    this.Builder.AppendFormat("{0} ", this.Dialect.THEN);
                    this.Visit(this.CreateConstant(1));
                    this.Builder.AppendFormat("{0} ", this.Dialect.ELSE);
                    this.Visit(this.CreateConstant(0));
                    this.Builder.AppendFormat("{0} ", this.Dialect.END);
                    break;
            }
        }

        protected override void VisitFunction(IFunctionBuilder expression)
        {
            switch (expression.Function)
            {
                case QueryFunction.Exists:
                    this.Builder.AppendFormat("{0} ", this.Dialect.CASE);
                    this.Builder.AppendFormat("{0} ", this.Dialect.WHEN);
                    base.VisitFunction(expression);
                    this.Builder.AppendFormat("{0} ", this.Dialect.THEN);
                    this.Visit(this.CreateConstant(1));
                    this.Builder.AppendFormat("{0} ", this.Dialect.ELSE);
                    this.Visit(this.CreateConstant(0));
                    this.Builder.AppendFormat("{0} ", this.Dialect.END);
                    break;
                default:
                    base.VisitFunction(expression);
                    break;
            }
        }
    }
}
