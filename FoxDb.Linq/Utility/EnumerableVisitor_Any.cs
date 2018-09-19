using FoxDb.Interfaces;
using System.Linq.Expressions;

namespace FoxDb
{
    public partial class EnumerableVisitor
    {
        protected virtual void VisitAny(MethodCallExpression node)
        {
            var relation = default(IRelationBuilder);
            if (this.TryCapture<IRelationBuilder>(null, node.Arguments[0], out relation))
            {
                this.VisitAny(node, relation.Relation);
            }
            else
            {
                this.VisitWhere(node);
            }
        }

        protected virtual void VisitAny(MethodCallExpression node, IRelationConfig relation)
        {
            var target = default(IFragmentTarget);
            if (!this.TryPeek(out target))
            {
                target = this.Query.Filter;
            }
            target.Write(target.CreateFunction(QueryFunction.Exists).With(function =>
            {
                var builder = this.Database.QueryFactory.Build();
                builder.Output.AddOperator(QueryOperator.Star);
                builder.Source.AddTable(relation.LeftTable.Extern());
                builder.Source.AddTable(relation.RightTable);
                builder.RelationManager.AddRelation(relation);
                function.AddArgument(function.CreateSubQuery(builder));
                this.Push(builder.Filter);
            }));
            try
            {
                for (var a = 1; a < node.Arguments.Count; a++)
                {
                    this.Visit(node.Arguments[a]);
                }
            }
            finally
            {
                this.Pop();
            }
        }
    }
}
