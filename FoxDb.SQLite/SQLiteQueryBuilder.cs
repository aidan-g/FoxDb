using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxDb
{
    public class SQLiteQueryBuilder : IQueryBuilder
    {
        public SQLiteQueryBuilder(IDatabase database, IQueryGraph graph)
        {
            this.Database = database;
            this.Graph = graph;
        }

        public IDatabase Database { get; private set; }

        public IQueryGraph Graph { get; private set; }

        public IDatabaseQuery Query
        {
            get
            {
                var visitor = new SQLiteQueryBuilderVisitor(this.Database);
                visitor.Visit(this.Graph);
                return visitor.Query;
            }
        }

        protected class SQLiteQueryBuilderVisitor : QueryGraphVisitor
        {
            protected override IDictionary<FragmentType, QueryGraphVisitorHandler> GetHandlers()
            {
                var handlers = base.GetHandlers();
                handlers[SQLiteQueryFragment.Limit] = fragment => this.VisitLimit(fragment as ILimitBuilder);
                handlers[SQLiteQueryFragment.Offset] = fragment => this.VisitOffset(fragment as IOffsetBuilder);
                return handlers;
            }

            private SQLiteQueryBuilderVisitor()
            {
                this.Members = new DynamicMethod(this.GetType());
                this.ParameterNames = new List<string>();
                this.Targets = new Stack<IFragmentTarget>();
                this.Fragments = new List<SQLiteQueryFragment>();
            }

            public SQLiteQueryBuilderVisitor(IDatabase database) : this()
            {
                this.Database = database;
            }

            protected DynamicMethod Members { get; private set; }

            public ICollection<string> ParameterNames { get; private set; }

            public IDatabase Database { get; private set; }

            public IDatabaseQuery Query
            {
                get
                {
                    return new DatabaseQuery(this.CommandText, this.ParameterNames.ToArray());
                }
            }

            public string CommandText
            {
                get
                {
                    var builder = new StringBuilder();
                    foreach (var fragment in this.Fragments.Prioritize())
                    {
                        builder.Append(fragment.CommandText);
                    }
                    return builder.ToString();
                }
            }

            protected Stack<IFragmentTarget> Targets { get; private set; }

            protected ICollection<SQLiteQueryFragment> Fragments { get; private set; }

            public IFragmentTarget Peek
            {
                get
                {
                    var target = this.Targets.Peek();
                    if (target == null)
                    {
                        throw new InvalidOperationException("No target to write fragment to.");
                    }
                    return target;
                }
            }

            public IFragmentTarget Push(IFragmentTarget target)
            {
                this.Targets.Push(target);
                return target;
            }

            public IFragmentTarget Pop()
            {
                var target = this.Targets.Pop();
                if (!string.IsNullOrEmpty(target.CommandText))
                {
                    this.Fragments.Add(new SQLiteQueryFragment(target));
                }
                return target;
            }

            protected override void VisitAdd(IAddBuilder expression)
            {
                this.Push(new SQLiteInsertWriter(this.Database, this, this.ParameterNames));
                this.Peek.Write(expression);
                this.Pop();
            }

            protected override void VisitUpdate(IUpdateBuilder expression)
            {
                this.Push(new SQLiteUpdateWriter(this.Database, this, this.ParameterNames));
                this.Peek.Write(expression);
                this.Pop();
            }

            protected override void VisitDelete(IDeleteBuilder expression)
            {
                this.Push(new SQLiteDeleteWriter(this.Database, this, this.ParameterNames));
                this.Peek.Write(expression);
                this.Pop();
            }

            protected override void VisitOutput(IOutputBuilder expression)
            {
                this.Push(new SQLiteSelectWriter(this.Database, this, this.ParameterNames));
                this.Peek.Write(expression);
                this.Pop();
            }

            protected override void VisitSource(ISourceBuilder expression)
            {
                this.Push(new SQLiteFromWriter(this.Database, this, this.ParameterNames));
                this.Peek.Write(expression);
                this.Pop();
            }

            protected override void VisitFilter(IFilterBuilder expression)
            {
                this.Push(new SQLiteWhereWriter(this.Database, this, this.ParameterNames));
                this.Peek.Write(expression);
                this.Pop();
            }

            protected override void VisitSort(ISortBuilder expression)
            {
                this.Push(new SQLiteOrderByWriter(this.Database, this, this.ParameterNames));
                this.Peek.Write(expression);
                this.Pop();
            }

            protected virtual void VisitLimit(ILimitBuilder expression)
            {
                this.Push(new SQLiteLimitWriter(this.Database, this, this.ParameterNames));
                this.Peek.Write(expression);
                this.Pop();
            }

            protected virtual void VisitOffset(IOffsetBuilder expression)
            {
                this.Push(new SQLiteOffsetWriter(this.Database, this, this.ParameterNames));
                this.Peek.Write(expression);
                this.Pop();
            }
        }
    }
}
