using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SqlCeSelectWriter : SqlSelectWriter
    {
        public SqlCeSelectWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters) : base(parent, graph, database, visitor, parameters)
        {
        }

        protected override T OnWrite<T>(T fragment)
        {
            if (fragment is IOutputBuilder)
            {
                var expression = fragment as IOutputBuilder;
                if (expression.Expressions.Any())
                {
                    this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.SELECT);
                    if (this.Graph.Filter.Limit != 0)
                    {
                        this.Builder.AppendFormat("{0} ", (this.Database.QueryFactory.Dialect as SqlCeQueryDialect).TOP);
                        this.Builder.AppendFormat("{0} ", this.Graph.Filter.Limit);
                        switch (this.Graph.Filter.LimitType)
                        {
                            case LimitType.None:
                                break;
                            case LimitType.Percent:
                                this.Builder.AppendFormat("{0} ", (this.Database.QueryFactory.Dialect as SqlCeQueryDialect).PERCENT);
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
                    this.Builder.AppendFormat("{0} ", (this.Database.QueryFactory.Dialect as SqlCeQueryDialect).CASE);
                    this.Builder.AppendFormat("{0} ", (this.Database.QueryFactory.Dialect as SqlCeQueryDialect).WHEN);
                    this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.NOT);
                    this.Visit(expression.Expression);
                    this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.EQUAL);
                    this.Builder.AppendFormat("{0} ", 1);
                    this.Builder.AppendFormat("{0} ", (this.Database.QueryFactory.Dialect as SqlCeQueryDialect).THEN);
                    this.Builder.AppendFormat("{0} ", 1);
                    this.Builder.AppendFormat("{0} ", (this.Database.QueryFactory.Dialect as SqlCeQueryDialect).ELSE);
                    this.Builder.AppendFormat("{0} ", 0);
                    this.Builder.AppendFormat("{0} ", (this.Database.QueryFactory.Dialect as SqlCeQueryDialect).END);
                    break;
            }
        }

        protected override void VisitFunction(IFunctionBuilder expression)
        {
            switch (expression.Function)
            {
                case QueryFunction.Exists:
                    this.Builder.AppendFormat("{0} ", (this.Database.QueryFactory.Dialect as SqlCeQueryDialect).CASE);
                    this.Builder.AppendFormat("{0} ", (this.Database.QueryFactory.Dialect as SqlCeQueryDialect).WHEN);
                    base.VisitFunction(expression);
                    this.Builder.AppendFormat("{0} ", (this.Database.QueryFactory.Dialect as SqlCeQueryDialect).THEN);
                    this.Builder.AppendFormat("{0} ", 1);
                    this.Builder.AppendFormat("{0} ", (this.Database.QueryFactory.Dialect as SqlCeQueryDialect).ELSE);
                    this.Builder.AppendFormat("{0} ", 0);
                    this.Builder.AppendFormat("{0} ", (this.Database.QueryFactory.Dialect as SqlCeQueryDialect).END);
                    break;
                default:
                    base.VisitFunction(expression);
                    break;
            }
        }
    }
}
