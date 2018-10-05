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
                    if (this.Graph.Filter.Limit.HasValue && !this.Graph.Filter.Offset.HasValue)
                    {
                        this.Builder.AppendFormat("{0} ", this.Dialect.TOP);
                        this.Builder.AppendFormat("{0} ", this.Graph.Filter.Limit);
                        switch (this.Graph.Filter.LimitType)
                        {
                            case LimitType.Percent:
                                this.Builder.AppendFormat("{0} ", this.Dialect.PERCENT);
                                break;
                        }
                    }
                    this.Visit(expression.Expressions);
                    return fragment;
                }
            }
            throw new NotImplementedException();
        }
    }
}
