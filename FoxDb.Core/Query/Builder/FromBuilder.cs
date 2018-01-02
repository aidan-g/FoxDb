using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class FromBuilder : FragmentBuilder, IFromBuilder
    {
        public FromBuilder()
        {
            this.Expressions = new List<IExpressionBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.From;
            }
        }

        public ICollection<IExpressionBuilder> Expressions { get; private set; }

        public void AddTable(ITableConfig table)
        {
            this.Expressions.Add(this.GetTable(table));
        }

        public void AddRelation(IRelationConfig relation)
        {
            this.Expressions.Add(this.GetRelation(relation));
        }

        public void AddSubQuery(IQueryGraphBuilder query)
        {
            this.Expressions.Add(this.GetSubQuery(query));
        }
    }
}
