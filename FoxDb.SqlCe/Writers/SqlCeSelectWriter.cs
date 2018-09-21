using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SqlCeSelectWriter : SqlSelectWriter
    {
        public SqlCeSelectWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {
            this.Dialect = this.Database.QueryFactory.Dialect as SqlCeQueryDialect;
        }

        public SqlCeQueryDialect Dialect { get; private set; }

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
                        this.Builder.AppendFormat("{0} ", this.Graph.Filter.Limit.Value);
                    }
                    this.Visit(expression.Expressions);
                }
                return fragment;
            }
            throw new NotImplementedException();
        }
    }
}
