using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxDb
{
    public abstract class SqlQueryRenderer : QueryGraphVisitor<IDatabaseQuery>
    {
        private SqlQueryRenderer() : base(QueryGraphVisitorFlags.None)
        {
            this.Parameters = new List<IDatabaseQueryParameter>();
            this.Targets = new Stack<IFragmentTarget>();
            this.Fragments = new List<SqlQueryFragment>();
        }

        public SqlQueryRenderer(IDatabase database)
            : this()
        {
            this.Database = database;
        }

        public ICollection<IDatabaseQueryParameter> Parameters { get; private set; }

        public IDatabase Database { get; private set; }

        public override IDatabaseQuery Result
        {
            get
            {
                return this.Database.QueryFactory.Create(this.CommandText, this.Parameters.ToArray());
            }
            protected set
            {
                throw new NotImplementedException();
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

        protected ICollection<SqlQueryFragment> Fragments { get; private set; }

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
                if (this.IsAssociativeExpression(target))
                {
                    this.WriteAssociativeExpression(target);
                }
                else
                {
                    this.Fragments.Add(this.CreateQueryFragment(target));
                }
            }
            return target;
        }

        protected virtual bool IsAssociativeExpression(IFragmentTarget target)
        {
            var writer = target as ISqlQueryWriter;
            if (writer == null)
            {
                return false;
            }
            return writer.GetRenderContext().HasFlag(RenderHints.AssociativeExpression);
        }

        protected virtual void WriteAssociativeExpression(IFragmentTarget target)
        {
            var writer = this.Peek as ISqlQueryWriter;
            if (writer == null)
            {
                throw new NotImplementedException();
            }
            writer.Builder.Append(target.CommandText);
        }

        protected override void VisitAdd(IFragmentBuilder parent, IQueryGraphBuilder graph, IAddBuilder expression)
        {
            this.Push(new SqlInsertWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitUpdate(IFragmentBuilder parent, IQueryGraphBuilder graph, IUpdateBuilder expression)
        {
            this.Push(new SqlUpdateWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitDelete(IFragmentBuilder parent, IQueryGraphBuilder graph, IDeleteBuilder expression)
        {
            this.Push(new SqlDeleteWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitOutput(IFragmentBuilder parent, IQueryGraphBuilder graph, IOutputBuilder expression)
        {
            this.Push(new SqlSelectWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitSource(IFragmentBuilder parent, IQueryGraphBuilder graph, ISourceBuilder expression)
        {
            this.Push(new SqlFromWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitFilter(IFragmentBuilder parent, IQueryGraphBuilder graph, IFilterBuilder expression)
        {
            this.Push(new SqlWhereWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitAggregate(IFragmentBuilder parent, IQueryGraphBuilder graph, IAggregateBuilder expression)
        {
            this.Push(new SqlGroupByWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitSort(IFragmentBuilder parent, IQueryGraphBuilder graph, ISortBuilder expression)
        {
            this.Push(new SqlOrderByWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitWith(IFragmentBuilder parent, IQueryGraphBuilder graph, IWithBuilder expression)
        {
            this.Push(new SqlWithWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitCreate(IFragmentBuilder parent, IQueryGraphBuilder graph, ICreateBuilder expression)
        {
            this.Push(new SqlCreateWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitAlter(IFragmentBuilder parent, IQueryGraphBuilder graph, IAlterBuilder expression)
        {
            this.Push(new SqlAlterWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitDrop(IFragmentBuilder parent, IQueryGraphBuilder graph, IDropBuilder expression)
        {
            this.Push(new SqlDropWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected abstract SqlQueryFragment CreateQueryFragment(IFragmentTarget target);
    }
}
