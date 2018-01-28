using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxDb
{
    public class SQLiteQueryBuilderVisitor : QueryGraphVisitor
    {
        protected override IDictionary<FragmentType, QueryGraphVisitorHandler> GetHandlers()
        {
            var handlers = base.GetHandlers();
            handlers[SQLiteQueryFragment.Limit] = (parent, graph, fragment) => this.VisitLimit(parent, graph, fragment as ILimitBuilder);
            handlers[SQLiteQueryFragment.Offset] = (parent, graph, fragment) => this.VisitOffset(parent, graph, fragment as IOffsetBuilder);
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

        protected override void VisitAdd(IFragmentBuilder parent, IQueryGraphBuilder graph, IAddBuilder expression)
        {
            this.Push(new SQLiteInsertWriter(parent, graph, this.Database, this, this.ParameterNames));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitUpdate(IFragmentBuilder parent, IQueryGraphBuilder graph, IUpdateBuilder expression)
        {
            this.Push(new SQLiteUpdateWriter(parent, graph, this.Database, this, this.ParameterNames));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitDelete(IFragmentBuilder parent, IQueryGraphBuilder graph, IDeleteBuilder expression)
        {
            this.Push(new SQLiteDeleteWriter(parent, graph, this.Database, this, this.ParameterNames));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitOutput(IFragmentBuilder parent, IQueryGraphBuilder graph, IOutputBuilder expression)
        {
            this.Push(new SQLiteSelectWriter(parent, graph, this.Database, this, this.ParameterNames));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitSource(IFragmentBuilder parent, IQueryGraphBuilder graph, ISourceBuilder expression)
        {
            this.Push(new SQLiteFromWriter(parent, graph, this.Database, this, this.ParameterNames));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitFilter(IFragmentBuilder parent, IQueryGraphBuilder graph, IFilterBuilder expression)
        {
            this.Push(new SQLiteWhereWriter(parent, graph, this.Database, this, this.ParameterNames));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitAggregate(IFragmentBuilder parent, IQueryGraphBuilder graph, IAggregateBuilder expression)
        {
            this.Push(new SQLiteGroupByWriter(parent, graph, this.Database, this, this.ParameterNames));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitSort(IFragmentBuilder parent, IQueryGraphBuilder graph, ISortBuilder expression)
        {
            this.Push(new SQLiteOrderByWriter(parent, graph, this.Database, this, this.ParameterNames));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected virtual void VisitLimit(IFragmentBuilder parent, IQueryGraphBuilder graph, ILimitBuilder expression)
        {
            this.Push(new SQLiteLimitWriter(parent, graph, this.Database, this, this.ParameterNames));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected virtual void VisitOffset(IFragmentBuilder parent, IQueryGraphBuilder graph, IOffsetBuilder expression)
        {
            this.Push(new SQLiteOffsetWriter(parent, graph, this.Database, this, this.ParameterNames));
            this.Peek.Write(expression);
            this.Pop();
        }
    }
}
