using FoxDb.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace FoxDb
{
    public partial class EnumerableVisitor
    {
        protected virtual void VisitContains(MethodCallExpression node)
        {
            var parameter = default(IParameterBuilder);
            var relation = default(IRelationBuilder);
            var context = default(CaptureFragmentContext);
            if (this.TryCapture<IParameterBuilder>(null, node.Arguments.Last(), out parameter, out context))
            {
                var element = context.Constants[parameter.Name];
                if (this.TryCapture<IRelationBuilder>(null, node.Object, out relation))
                {
                    this.VisitContains(node, relation.Relation, element);
                }
                else
                {
                    this.VisitContains(node, element);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected virtual void VisitContains(MethodCallExpression node, object element)
        {
            var pop = false;
            if (!this.TryPeek())
            {
                this.Push(this.Query.Filter);
                pop = true;
            }
            try
            {
                this.Peek.Write(
                    this.Push(this.Peek.CreateBinary(
                        this.Peek.CreateColumn(this.Table.PrimaryKey),
                        QueryOperator.Equal,
                        //Leave the right expression null, we will write to this later.
                        null
                    )).With(binary =>
                    {
                        try
                        {
                            this.VisitParameter(this.Table.PrimaryKey.ColumnType.Type, this.Table.PrimaryKey.Getter(element));
                        }
                        finally
                        {
                            this.Pop();
                        }
                    })
                );
            }
            finally
            {
                if (pop)
                {
                    this.Pop();
                }
            }
        }

        protected virtual void VisitContains(MethodCallExpression node, IRelationConfig relation, object element)
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
                this.Peek.Write(
                    this.Push(this.Peek.CreateBinary(
                        this.Peek.CreateColumn(relation.RightTable.PrimaryKey),
                        QueryOperator.Equal,
                        //Leave the right expression null, we will write to this later.
                        null
                    )).With(binary =>
                    {
                        try
                        {
                            this.VisitParameter(relation.RightTable.PrimaryKey.ColumnType.Type, relation.RightTable.PrimaryKey.Getter(element));
                        }
                        finally
                        {
                            this.Pop();
                        }
                    })
                );
            }
            finally
            {
                this.Pop();
            }
        }
    }
}
