using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SQLiteFromWriter : SQLiteQueryWriter
    {
        public SQLiteFromWriter(IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(database, visitor, parameterNames)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Source;
            }
        }

        public override T Write<T>(T fragment)
        {
            if (fragment is ISourceBuilder)
            {
                var expression = fragment as ISourceBuilder;
                if (expression.Expressions.Any())
                {
                    this.Builder.AppendFormat("{0} ", SQLiteSyntax.FROM);
                    this.Visit(expression.Expressions);
                }
                return fragment;
            }
            throw new NotImplementedException();
        }

        protected override void Visit(IEnumerable<IExpressionBuilder> expressions)
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
                    if (expression is ITableBuilder || expression is ISubQueryBuilder)
                    {
                        this.Builder.AppendFormat("{0} ", SQLiteSyntax.LIST_DELIMITER);
                    }
                    else if (expression is IRelationBuilder)
                    {
                        //Nothing to do.
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                this.Visit(expression);
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
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.OPEN_PARENTHESES);
            base.VisitSubQuery(expression);
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.CLOSE_PARENTHESES);
            this.VisitAlias(expression.Alias);
        }
    }
}
