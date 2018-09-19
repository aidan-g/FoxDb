using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FoxDb
{
    public partial class EnumerableVisitor
    {
        protected virtual T Capture<T>(IFragmentBuilder parent, Expression node) where T : IFragmentBuilder
        {
            var expression = default(T);
            if (!this.TryCapture<T>(parent, node, out expression))
            {
                throw new InvalidOperationException(string.Format("Failed to capture fragment of type \"{0}\".", typeof(T).FullName));
            }
            return expression;
        }

        protected virtual T Capture<T>(IFragmentBuilder parent, Expression node, out CaptureFragmentContext context) where T : IFragmentBuilder
        {
            var expression = default(T);
            if (!this.TryCapture<T>(parent, node, out expression, out context))
            {
                throw new InvalidOperationException(string.Format("Failed to capture fragment of type \"{0}\".", typeof(T).FullName));
            }
            return expression;
        }

        protected virtual bool TryCapture<T>(IFragmentBuilder parent, Expression node, out T result) where T : IFragmentBuilder
        {
            var context = default(CaptureFragmentContext);
            if (!this.TryCapture<T>(parent, node, out result, out context))
            {
                return false;
            }
            if (context.Constants.Any())
            {
                throw new InvalidOperationException("Capture resulted in unhandled constants.");
            }
            return true;
        }

        protected virtual bool TryCapture<T>(IFragmentBuilder parent, Expression node, out T result, out CaptureFragmentContext context) where T : IFragmentBuilder
        {
            context = new CaptureFragmentContext(parent, this.Query.Clone());
            var capture = new CaptureFragmentTarget(context);
            this.Push(capture);
            try
            {
                this.Visit(node);
            }
            finally
            {
                this.Pop(false);
            }
            foreach (var expression in context.Expressions)
            {
                if (expression is T)
                {
                    result = (T)expression;
                    return true;
                }
            }
            result = default(T);
            return false;
        }

        protected class CaptureFragmentTarget : FragmentBuilder, IFragmentTarget
        {
            public CaptureFragmentTarget(CaptureFragmentContext context)
                : base(context.Parent, context.Graph)
            {
                this.Context = context;
            }

            public override FragmentType FragmentType
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IDictionary<string, object> Constants
            {
                get
                {
                    return this.Context.Constants;
                }
            }

            public CaptureFragmentContext Context { get; private set; }

            public T Write<T>(T fragment) where T : IFragmentBuilder
            {
                this.Context.Expressions.Add(fragment);
                return fragment;
            }

            public override IFragmentBuilder Clone()
            {
                throw new NotImplementedException();
            }
        }

        public class CaptureFragmentContext
        {
            private CaptureFragmentContext()
            {
                this.Expressions = new List<IFragmentBuilder>();
                this.Constants = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }

            public CaptureFragmentContext(IFragmentBuilder parent, IQueryGraphBuilder graph)
                : this()
            {
                this.Parent = parent;
                this.Graph = graph;
            }

            public IFragmentBuilder Parent { get; private set; }

            public IQueryGraphBuilder Graph { get; private set; }

            public ICollection<IFragmentBuilder> Expressions { get; private set; }

            public IDictionary<string, object> Constants { get; private set; }

            public bool IsEmpty
            {
                get
                {
                    return this.Expressions.Count == 0 && this.Constants.Count == 0;
                }
            }
        }
    }
}
