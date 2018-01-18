using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SourceBuilder : FragmentBuilder, ISourceBuilder
    {
        public SourceBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(graph)
        {
            this.Parent = parent;
            this.Expressions = new List<IExpressionBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Source;
            }
        }

        public IFragmentBuilder Parent { get; private set; }

        public ICollection<IExpressionBuilder> Expressions { get; private set; }

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

        public IRelationBuilder GetRelation(IRelationConfig relation)
        {
            return this.GetExpression<IRelationBuilder>(builder => builder.Relation == relation);
        }

        public IRelationBuilder AddRelation(IRelationConfig relation)
        {
            var builder = this.CreateRelation(relation);
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
    }
}
