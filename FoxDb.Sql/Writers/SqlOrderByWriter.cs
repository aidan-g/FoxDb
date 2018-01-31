using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SqlGroupByWriter : SqlQueryWriter
    {
        public SqlGroupByWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(parent, graph, database, visitor, parameterNames)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Aggregate;
            }
        }

        protected override T OnWrite<T>(T fragment)
        {
            if (fragment is IAggregateBuilder)
            {
                var expression = fragment as IAggregateBuilder;
                if (expression.Expressions.Any())
                {
                    this.Builder.AppendFormat("{0} ", SqlSyntax.GROUP_BY);
                    this.Visit(expression.Expressions);
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
                    this.Builder.AppendFormat("{0} ", SqlSyntax.LIST_DELIMITER);
                }
                this.Visit(expression);
            }
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
