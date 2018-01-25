using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxDb
{
    public abstract class SQLiteQueryWriter : FragmentBuilder, ISQLiteQueryWriter
    {
        protected IDictionary<FragmentType, QueryGraphVisitorHandler> Handlers { get; private set; }

        protected Stack<IFragmentBuilder> FragmentContext { get; private set; }

        protected Stack<RenderHints> RenderContext { get; private set; }

        #region ISQLiteQueryWriter

        IReadOnlyCollection<IFragmentBuilder> ISQLiteQueryWriter.FragmentContext
        {
            get
            {
                return this.FragmentContext;
            }
        }

        public T GetFragmentContext<T>() where T : IFragmentBuilder
        {
            return this.FragmentContext.OfType<T>().FirstOrDefault();
        }

        public IFragmentBuilder GetFragmentContext()
        {
            if (this.FragmentContext.Count == 0)
            {
                return default(IFragmentBuilder);
            }
            return this.FragmentContext.Peek();
        }

        public T AddFragmentContext<T>(T context) where T : IFragmentBuilder
        {
            this.FragmentContext.Push(context);
            return context;
        }

        public T RemoveFragmentContext<T>() where T : IFragmentBuilder
        {
            var context = this.FragmentContext.Peek();
            if (!(context is T))
            {
                return default(T);
            }
            return (T)this.FragmentContext.Pop();
        }

        public IFragmentBuilder RemoveFragmentContext()
        {
            return this.FragmentContext.Pop();
        }

        IReadOnlyCollection<RenderHints> ISQLiteQueryWriter.RenderContext
        {
            get
            {
                return this.RenderContext;
            }
        }

        public RenderHints GetRenderContext()
        {
            if (this.RenderContext.Count == 0)
            {
                return RenderHints.None;
            }
            return this.RenderContext.Peek();
        }

        public RenderHints AddRenderContext(RenderHints context)
        {
            this.RenderContext.Push(context);
            return context;
        }

        public RenderHints RemoveRenderContext()
        {
            return this.RenderContext.Pop();
        }

        #endregion

        protected StringBuilder Builder { get; private set; }

        protected virtual IDictionary<FragmentType, QueryGraphVisitorHandler> GetHandlers()
        {
            return new Dictionary<FragmentType, QueryGraphVisitorHandler>()
            {
                { FragmentType.Binary, (parent, fragment) => this.VisitBinary(fragment as IBinaryExpressionBuilder) },
                { FragmentType.Table, (parent, fragment) => this.VisitTable(fragment as ITableBuilder) },
                { FragmentType.Column, (parent, fragment) => this.VisitColumn(fragment as IColumnBuilder) },
                { FragmentType.Parameter, (parent, fragment) => this.VisitParameter(fragment as IParameterBuilder) },
                { FragmentType.Function, (parent, fragment) => this.VisitFunction(fragment as IFunctionBuilder) },
                { FragmentType.Operator, (parent, fragment) => this.VisitOperator(fragment as IOperatorBuilder) },
                { FragmentType.Constant, (parent, fragment) => this.VisitConstant(fragment as IConstantBuilder) },
                { FragmentType.SubQuery, (parent, fragment) => this.VisitSubQuery(fragment as ISubQueryBuilder) }
            };
        }

        protected static IDictionary<QueryOperator, string> Operators = new Dictionary<QueryOperator, string>()
        {
            { QueryOperator.Equal, SQLiteSyntax.EQUAL },
            { QueryOperator.NotEqual, SQLiteSyntax.NOT_EQUAL },
            { QueryOperator.Greater, SQLiteSyntax.GREATER },
            { QueryOperator.Less, SQLiteSyntax.LESS },
            { QueryOperator.And, SQLiteSyntax.AND },
            { QueryOperator.AndAlso, SQLiteSyntax.AND_ALSO },
            { QueryOperator.Or, SQLiteSyntax.OR },
            { QueryOperator.OrElse, SQLiteSyntax.OR_ELSE },
            { QueryOperator.OpenParentheses, SQLiteSyntax.OPEN_PARENTHESES },
            { QueryOperator.CloseParentheses, SQLiteSyntax.CLOSE_PARENTHESES },
            { QueryOperator.Null, SQLiteSyntax.NULL },
            { QueryOperator.Star, SQLiteSyntax.STAR }
        };

        protected static IDictionary<QueryFunction, string> Functions = new Dictionary<QueryFunction, string>()
        {
            { QueryFunction.Identity, SQLiteSyntax.IDENTITY },
            { QueryFunction.Count, SQLiteSyntax.COUNT },
            { QueryFunction.Exists, SQLiteSyntax.EXISTS }
        };

        protected SQLiteQueryWriter(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {
            this.Handlers = this.GetHandlers();
            if (parent is ISQLiteQueryWriter)
            {
                this.FragmentContext = new Stack<IFragmentBuilder>((parent as ISQLiteQueryWriter).FragmentContext);
                this.RenderContext = new Stack<RenderHints>((parent as ISQLiteQueryWriter).RenderContext);
            }
            else
            {
                this.FragmentContext = new Stack<IFragmentBuilder>();
                this.RenderContext = new Stack<RenderHints>();
            }
            this.Builder = new StringBuilder();
        }

        public SQLiteQueryWriter(IFragmentBuilder parent, IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : this(parent, QueryGraphBuilder.Null)
        {
            this.Database = database;
            this.Visitor = visitor;
            this.ParameterNames = parameterNames;
        }

        public IDatabase Database { get; private set; }

        public IQueryGraphVisitor Visitor { get; private set; }

        public ICollection<string> ParameterNames { get; private set; }

        public override FragmentType FragmentType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string CommandText
        {
            get
            {
                return this.Builder.ToString();
            }
        }

        public IDictionary<string, object> Constants
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public T Write<T>(T fragment) where T : IFragmentBuilder
        {
            this.AddFragmentContext(fragment);
            try
            {
                return this.OnWrite(fragment);
            }
            finally
            {
                this.RemoveFragmentContext();
            }
        }

        protected abstract T OnWrite<T>(T fragment) where T : IFragmentBuilder;

        protected virtual void Visit(IFragmentBuilder expression)
        {
            var handler = default(QueryGraphVisitorHandler);
            if (!this.Handlers.TryGetValue(expression.FragmentType, out handler))
            {
                throw new NotImplementedException();
            }
            this.AddFragmentContext(expression);
            try
            {
                handler(this, expression);
            }
            finally
            {
                this.RemoveFragmentContext();
            }
        }

        protected virtual void Visit(IEnumerable<IFragmentBuilder> expressions)
        {
            foreach (var expression in expressions)
            {
                this.Visit(expression);
            }
        }

        protected virtual void VisitTable(ITableBuilder expression)
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.Identifier(expression.Table.TableName));
        }

        protected virtual void VisitBinary(IBinaryExpressionBuilder expression)
        {
            this.Visit(expression.Left);
            this.Visit(expression.Operator);
            this.Visit(expression.Right);
        }

        protected virtual void VisitColumn(IColumnBuilder expression)
        {
            var identifier = default(string);
            if (expression.Flags.HasFlag(ColumnBuilderFlags.Identifier))
            {
                identifier = expression.Column.Identifier;
            }
            else
            {
                identifier = expression.Column.ColumnName;
            }
            if (expression.Flags.HasFlag(ColumnBuilderFlags.Distinct))
            {
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.DISTINCT);
            }
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.Identifier(expression.Column.Table.TableName, identifier));
        }

        protected virtual void VisitParameter(IParameterBuilder expression)
        {
            this.Builder.AppendFormat("{0}{1} ", SQLiteSyntax.PARAMETER, expression.Name);
            this.ParameterNames.Add(expression.Name);
        }

        protected virtual void VisitFunction(IFunctionBuilder expression)
        {
            var function = default(string);
            if (!Functions.TryGetValue(expression.Function, out function))
            {
                throw new NotImplementedException();
            }
            this.Builder.AppendFormat("{0} ", function);
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.OPEN_PARENTHESES);
            if (expression.Expressions.Any())
            {
                this.AddRenderContext(RenderHints.FunctionArgument);
                try
                {
                    this.Visit(expression.Expressions);
                }
                finally
                {
                    this.RemoveRenderContext();
                }
            }
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.CLOSE_PARENTHESES);
        }

        protected virtual void VisitOperator(IOperatorBuilder expression)
        {
            var @operator = default(string);
            if (!Operators.TryGetValue(expression.Operator, out @operator))
            {
                throw new NotImplementedException();
            }
            this.Builder.AppendFormat("{0} ", @operator);
        }

        protected virtual void VisitConstant(IConstantBuilder expression)
        {
            if (expression.Value == null)
            {
                this.Visit(expression.CreateOperator(QueryOperator.Null));
            }
            else
            {
                switch (Type.GetTypeCode(expression.Value.GetType()))
                {
                    case TypeCode.Int32:
                        this.Builder.AppendFormat("{0} ", expression.Value);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        protected virtual void VisitSubQuery(ISubQueryBuilder expression)
        {
            var query = expression.Query.Build();
            this.Builder.AppendFormat("{0} ", query.CommandText);
            foreach (var parameterName in query.ParameterNames)
            {
                if (this.ParameterNames.Contains(parameterName, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }
                this.ParameterNames.Add(parameterName);
            }
        }

        protected virtual void VisitAlias(string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                return;
            }
            this.Builder.AppendFormat("{0} {1} ", SQLiteSyntax.AS, SQLiteSyntax.Identifier(alias));
        }
    }

    [Flags]
    public enum RenderHints : byte
    {
        None = 0,
        FunctionArgument = 1
    }
}
