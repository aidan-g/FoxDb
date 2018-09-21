using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SqlCeWhereWriter : SqlWhereWriter
    {
        public SqlCeWhereWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {
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
                if (expression.Offset.HasValue)
                {
                    this.Visitor.Visit(this, this.Graph, new OffsetBuilder(this, this.Graph, expression.Offset.Value));
                    if (expression.Limit.HasValue)
                    {
                        this.Visitor.Visit(this, this.Graph, new LimitBuilder(this, this.Graph, expression.Limit.Value));
                    }
                }
                return fragment;
            }
            throw new NotImplementedException();
        }
    }
}
