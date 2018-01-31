using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SQLiteWhereWriter : SqlWhereWriter
    {
        public SQLiteWhereWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(parent, graph, database, visitor, parameterNames)
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
                    this.Builder.AppendFormat("{0} ", SqlSyntax.WHERE);
                    if (expression.Expressions.Any())
                    {
                        this.Visit(expression.Expressions);
                        first = false;
                    }
                    this.Visit(this.Graph.RelationManager.Calculator, this.Graph.RelationManager.CalculatedRelations, first);
                }
                if (expression.Limit != 0)
                {
                    this.Visitor.Visit(this, this.Graph, new LimitBuilder(this, this.Graph, expression.Limit));
                }
                if (expression.Offset != 0)
                {
                    this.Visitor.Visit(this, this.Graph, new OffsetBuilder(this, this.Graph, expression.Offset));
                }
                return fragment;
            }
            throw new NotImplementedException();
        }
    }
}
