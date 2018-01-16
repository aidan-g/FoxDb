using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SourceBuilder : FragmentBuilder, ISourceBuilder
    {
        public SourceBuilder()
        {
            this.Expressions = new List<IExpressionBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Source;
            }
        }

        public ICollection<IExpressionBuilder> Expressions { get; private set; }

        public ITableBuilder AddTable(ITableConfig table)
        {
            var builder = this.GetTable(table);
            this.Expressions.Add(builder);
            return builder;
        }

        public IRelationBuilder AddRelation(IRelationConfig relation)
        {
            var builder = this.GetRelation(relation);
            this.Expressions.Add(builder);
            return builder;
        }

        public ISubQueryBuilder AddSubQuery(IQueryGraphBuilder query)
        {
            var builder = this.GetSubQuery(query);
            this.Expressions.Add(builder);
            return builder;
        }
    }
}
