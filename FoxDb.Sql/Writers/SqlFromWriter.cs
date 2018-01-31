using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SqlFromWriter : SqlQueryWriter
    {
        public SqlFromWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(parent, graph, database, visitor, parameterNames)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Source;
            }
        }

        protected override T OnWrite<T>(T fragment)
        {
            if (fragment is ISourceBuilder)
            {
                var expression = fragment as ISourceBuilder;
                if (expression.Expressions.Any())
                {
                    this.Builder.AppendFormat("{0} ", SqlSyntax.FROM);
                    this.Visit(expression.Expressions);
                    this.Visit(this.Graph.RelationManager.CalculatedRelations);
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
                var table = expression as ITableBuilder;
                if (table != null)
                {
                    if (table.Table.Flags.HasFlag(TableFlags.Extern))
                    {
                        //Nothing to do. Table is used for relation hinting only.
                        continue;
                    }
                }
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

        protected virtual void Visit(IEnumerable<IEntityRelation> relations)
        {
            foreach (var relation in relations)
            {
                if (relation.IsExternal)
                {
                    //Nothing to do. Table is used for relation hinting only.
                    continue;
                }
                else
                {
                    this.Visit(relation.Table, relation.Expression);
                }
            }
        }

        protected override void VisitTable(ITableBuilder expression)
        {
            if (expression.Filter.Limit == 0 && expression.Filter.Offset == 0 && expression.Filter.IsEmpty() && expression.Sort.IsEmpty())
            {
                base.VisitTable(expression);
            }
            else
            {
                //If the table builder is filtered or sorted we have to create a sub query encapsulating this logic.
                //This is really only necessary if limit or offset is defined.
                var builder = this.Database.QueryFactory.Build();
                builder.Output.AddOperator(QueryOperator.Star);
                builder.Source.AddTable(expression.Table);
                builder.Filter.Limit = expression.Filter.Limit;
                builder.Filter.Offset = expression.Filter.Offset;
                builder.Filter.Expressions.AddRange(expression.Filter.Expressions);
                builder.Sort.Expressions.AddRange(expression.Sort.Expressions);
                this.VisitSubQuery(this.CreateSubQuery(builder).With(
                    query => query.Alias = expression.Table.TableName
                ));
            }
        }

        protected override void VisitSubQuery(ISubQueryBuilder expression)
        {
            this.Builder.AppendFormat("{0} ", SqlSyntax.OPEN_PARENTHESES);
            base.VisitSubQuery(expression);
            this.Builder.AppendFormat("{0} ", SqlSyntax.CLOSE_PARENTHESES);
            this.VisitAlias(expression.Alias);
        }

        protected override void VisitUnary(IUnaryExpressionBuilder expression)
        {
            this.Builder.AppendFormat("{0} ", SqlSyntax.OPEN_PARENTHESES);
            base.VisitUnary(expression);
            this.Builder.AppendFormat("{0} ", SqlSyntax.CLOSE_PARENTHESES);
        }

        protected override void VisitBinary(IBinaryExpressionBuilder expression)
        {
            this.Builder.AppendFormat("{0} ", SqlSyntax.OPEN_PARENTHESES);
            base.VisitBinary(expression);
            this.Builder.AppendFormat("{0} ", SqlSyntax.CLOSE_PARENTHESES);
        }

        protected virtual void Visit(ITableConfig table, IBinaryExpressionBuilder expression)
        {
            this.Builder.AppendFormat("{0} ", SqlSyntax.JOIN);
            this.VisitTable(this.CreateTable(table));
            this.Builder.AppendFormat("{0} ", SqlSyntax.ON);
            this.Visit(expression);
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
