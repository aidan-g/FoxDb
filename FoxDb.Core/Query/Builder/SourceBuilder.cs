using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SourceBuilder : FragmentBuilder, ISourceBuilder
    {
        public SourceBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {
            this.Expressions = new List<IFragmentBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Source;
            }
        }

        public ICollection<IFragmentBuilder> Expressions { get; private set; }

        public IEnumerable<ITableBuilder> Tables
        {
            get
            {
                return this.Expressions.OfType<ITableBuilder>();
            }
        }

        public IEnumerable<ISubQueryBuilder> SubQueries
        {
            get
            {
                return this.Expressions.OfType<ISubQueryBuilder>();
            }
        }

        public ITableBuilder GetTable(ITableConfig table)
        {
            return this.GetExpression<ITableBuilder>(builder => builder.Table == table);
        }

        public ITableBuilder AddTable(ITableConfig table)
        {
            var builder = this.CreateTable(table);
            this.Expressions.Add(builder);
            return builder;
        }

        public ISubQueryBuilder GetSubQuery(IQueryGraphBuilder query)
        {
            return this.GetExpression<ISubQueryBuilder>(builder => builder.Query == query);
        }

        public ISubQueryBuilder AddSubQuery(IQueryGraphBuilder query)
        {
            var builder = this.CreateSubQuery(query);
            this.Expressions.Add(builder);
            return builder;
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<ISourceBuilder>().With(builder =>
            {
                foreach (var expression in this.Expressions)
                {
                    builder.Expressions.Add(expression.Clone());
                }
            });
        }

        public override string DebugView
        {
            get
            {
                return string.Format("{{{0}}}", string.Join(", ", this.Expressions.Select(expression => expression.DebugView)));
            }
        }
    }
}
