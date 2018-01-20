using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SQLiteFromWriter : SQLiteQueryWriter
    {
        public SQLiteFromWriter(IFragmentBuilder parent, IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(parent, database, visitor, parameterNames)
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
                    this.Builder.AppendFormat("{0} ", SQLiteSyntax.FROM);
                    var writer = this.Push(new SQLiteJoinWriter(this, this.Database, this.Visitor, this.ParameterNames));
                    try
                    {
                        this.Visit(expression.Expressions);
                    }
                    finally
                    {
                        this.Pop();
                    }
                    this.Visit(writer);
                }
                return fragment;
            }
            throw new NotImplementedException();
        }

        protected override void Visit(IEnumerable<IFragmentBuilder> expressions)
        {
            var first = true;
            var relations = new List<IRelationBuilder>();
            foreach (var expression in expressions)
            {
                if (expression is IRelationBuilder)
                {
                    relations.Add(expression as IRelationBuilder);
                    continue;
                }
                if (first)
                {
                    first = false;
                }
                else
                {
                    if (expression is ITableBuilder || expression is ISubQueryBuilder)
                    {
                        this.Builder.AppendFormat("{0} ", SQLiteSyntax.LIST_DELIMITER);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                this.Visit(expression);
            }
            foreach (var relation in relations)
            {
                this.VisitRelation(relation);
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

        protected virtual void VisitRelation(IRelationBuilder expression)
        {
            this.GetContext<SQLiteJoinWriter>().Write(expression);
        }

        protected override void VisitSubQuery(ISubQueryBuilder expression)
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.OPEN_PARENTHESES);
            base.VisitSubQuery(expression);
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.CLOSE_PARENTHESES);
            this.VisitAlias(expression.Alias);
        }

        protected override void VisitBinary(IBinaryExpressionBuilder expression)
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.OPEN_PARENTHESES);
            base.VisitBinary(expression);
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.CLOSE_PARENTHESES);
        }

        protected virtual void Visit(SQLiteJoinWriter writer)
        {
            var tables = new List<ITableConfig>();
            tables.AddRange(this.GetContext<ISourceBuilder>().Tables.Select(builder => builder.Table));
            foreach (var key in writer.Keys.Prioritize(this))
            {
                var expression = writer.GetExpression(key);
                foreach (var table in key)
                {
                    if (tables.Contains(table))
                    {
                        var other = key.Except(new[] { table }).Single();
                        this.Visit(other, expression);
                        tables.Add(other);
                        break;
                    }
                }
            }
        }

        protected virtual void Visit(ITableConfig table, IBinaryExpressionBuilder expression)
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.JOIN);
            this.VisitTable(this.CreateTable(table));
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.ON);
            this.Visit(expression);
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
