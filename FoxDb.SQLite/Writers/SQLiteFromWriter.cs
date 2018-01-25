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
                    var writer = this.AddFragmentContext(new SQLiteJoinWriter(this, this.Database, this.Visitor, this.ParameterNames));
                    try
                    {
                        this.Visit(expression.Expressions);
                    }
                    finally
                    {
                        this.RemoveFragmentContext();
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
            this.GetFragmentContext<SQLiteJoinWriter>().Write(expression);
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
            if (!writer.Keys.Any())
            {
                //Nothing to do.
                return;
            }
            var tables = new List<ITableConfig>(
                this.GetFragmentContext<ISourceBuilder>().Tables.Select(builder => builder.Table)
            );
            foreach (var key in this.GetKeysByDependency(writer))
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

        /// <summary>
        /// Return the keys in the writer ordered by their dependency on each other.
        /// A join cannot reference a table which is included later in the query.
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        protected virtual IEnumerable<ITableConfigContainer> GetKeysByDependency(SQLiteJoinWriter writer)
        {
            //It doesn't look like it, but this is really hard.
            var result = new List<ITableConfigContainer>();
            var remaining = writer.Keys.ToList();
            var source = writer.GetFragmentContext<ISourceBuilder>();
            var tables = source.Tables.Select(table => table.Table);
            //First things first, we add all "simple" joins which are already satisfied by tables included in the query source.
            foreach (var key in remaining.ToArray())
            {
                if (tables.Contains(key))
                {
                    result.Add(key);
                    remaining.Remove(key);
                }
            }
            //While we have unresolved keys;
            while (remaining.Count > 0)
            {
                var success = false;
                //Buffer the sequence because we modify it.
                foreach (var key in remaining.ToArray())
                {
                    var relation = writer.GetRelation(key);
                    //Available tables are;
                    var available = tables //Tables from the query source.
                        .Concat(result.SelectMany<ITableConfigContainer, ITableConfig>()) //Already resolved keys.
                        .Concat(relation.MappingTable, relation.RightTable) //The mapping and right tables of the associated relation.
                        .Distinct(); //Normalize.
                    if (available.Contains(key))
                    {
                        //Looks like this join is satisfied, stage and remove from the queue.
                        result.Add(key);
                        remaining.Remove(key);
                        success = true;
                    }
                }
                //We could not detect which join comes next :(
                if (!success)
                {
                    //Just add the remaining joins and hope for the best.
                    //This is totally wrong.
                    result.AddRange(remaining);
                    remaining.Clear();
                    break;
                }
            }
            return result;
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
