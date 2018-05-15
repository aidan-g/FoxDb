using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace FoxDb
{
    public abstract class SqlQueryWriter : FragmentBuilder, ISqlQueryWriter
    {
        protected static IDictionary<FragmentType, SqlQueryWriterVisitorHandler> Handlers = new Dictionary<FragmentType, SqlQueryWriterVisitorHandler>()
        {
            { FragmentType.Unary, (writer, fragment) => writer.VisitUnary(fragment as IUnaryExpressionBuilder) },
            { FragmentType.Binary, (writer, fragment) => writer.VisitBinary(fragment as IBinaryExpressionBuilder) },
            { FragmentType.Table, (writer, fragment) => writer.VisitTable(fragment as ITableBuilder) },
            { FragmentType.Column, (writer, fragment) => writer.VisitColumn(fragment as IColumnBuilder) },
            { FragmentType.Parameter, (writer, fragment) => writer.VisitParameter(fragment as IParameterBuilder) },
            { FragmentType.Function, (writer, fragment) => writer.VisitFunction(fragment as IFunctionBuilder) },
            { FragmentType.Operator, (writer, fragment) => writer.VisitOperator(fragment as IOperatorBuilder) },
            { FragmentType.Constant, (writer, fragment) => writer.VisitConstant(fragment as IConstantBuilder) },
            { FragmentType.SubQuery, (writer, fragment) => writer.VisitSubQuery(fragment as ISubQueryBuilder) },
            { FragmentType.Sequence, (writer, fragment) => writer.VisitSequence(fragment as ISequenceBuilder) }
        };

        protected Stack<IFragmentBuilder> FragmentContext { get; private set; }

        protected Stack<RenderHints> RenderContext { get; private set; }

        #region ISqlQueryWriter

        IReadOnlyCollection<IFragmentBuilder> ISqlQueryWriter.FragmentContext
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

        IReadOnlyCollection<RenderHints> ISqlQueryWriter.RenderContext
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

        protected static IDictionary<QueryOperator, SqlQueryWriterDialectHandler> Operators = new Dictionary<QueryOperator, SqlQueryWriterDialectHandler>()
        {
            //Logical
            { QueryOperator.Not, writer => writer.Database.QueryFactory.Dialect.NOT },
            { QueryOperator.Is,  writer => writer.Database.QueryFactory.Dialect.IS },
            { QueryOperator.In, writer => writer.Database.QueryFactory.Dialect.IN },
            { QueryOperator.Equal, writer => writer.Database.QueryFactory.Dialect.EQUAL },
            { QueryOperator.NotEqual, writer => writer.Database.QueryFactory.Dialect.NOT_EQUAL },
            { QueryOperator.Greater, writer => writer.Database.QueryFactory.Dialect.GREATER },
            { QueryOperator.GreaterOrEqual, writer => writer.Database.QueryFactory.Dialect.GREATER_OR_EQUAL },
            { QueryOperator.Less, writer => writer.Database.QueryFactory.Dialect.LESS },
            { QueryOperator.LessOrEqual, writer => writer.Database.QueryFactory.Dialect.LESS_OR_EQUAL },
            { QueryOperator.And, writer => writer.Database.QueryFactory.Dialect.AND },
            { QueryOperator.AndAlso, writer => writer.Database.QueryFactory.Dialect.AND_ALSO },
            { QueryOperator.Or, writer => writer.Database.QueryFactory.Dialect.OR },
            { QueryOperator.OrElse, writer => writer.Database.QueryFactory.Dialect.OR_ELSE },
            { QueryOperator.OpenParentheses, writer => writer.Database.QueryFactory.Dialect.OPEN_PARENTHESES },
            { QueryOperator.CloseParentheses, writer => writer.Database.QueryFactory.Dialect.CLOSE_PARENTHESES },
            //Mathematical
            { QueryOperator.Add, writer => writer.Database.QueryFactory.Dialect.ADD },
            //Other
            { QueryOperator.Null, writer => writer.Database.QueryFactory.Dialect.NULL },
            { QueryOperator.Star, writer => writer.Database.QueryFactory.Dialect.STAR }
        };

        protected static IDictionary<QueryFunction, SqlQueryWriterDialectHandler> Functions = new Dictionary<QueryFunction, SqlQueryWriterDialectHandler>()
        {
            { QueryFunction.Count, writer => writer.Database.QueryFactory.Dialect.COUNT },
            { QueryFunction.Exists, writer => writer.Database.QueryFactory.Dialect.EXISTS }
        };

        protected SqlQueryWriter(IFragmentBuilder parent, IQueryGraphBuilder graph)
            : base(parent, graph)
        {
            if (parent is ISqlQueryWriter)
            {
                this.FragmentContext = new Stack<IFragmentBuilder>((parent as ISqlQueryWriter).FragmentContext);
                this.RenderContext = new Stack<RenderHints>((parent as ISqlQueryWriter).RenderContext);
            }
            else
            {
                this.FragmentContext = new Stack<IFragmentBuilder>();
                this.RenderContext = new Stack<RenderHints>();
            }
            this.Builder = new StringBuilder();
        }

        protected SqlQueryWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : this(parent, graph)
        {
            this.Database = database;
            this.Visitor = visitor;
            this.Parameters = parameters;
        }

        public IDatabase Database { get; private set; }

        public IQueryGraphVisitor Visitor { get; private set; }

        public ICollection<IDatabaseQueryParameter> Parameters { get; private set; }

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
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            var handler = default(SqlQueryWriterVisitorHandler);
            if (!Handlers.TryGetValue(expression.FragmentType, out handler))
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
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.Identifier(expression.Table.TableName));
        }

        protected virtual void VisitUnary(IUnaryExpressionBuilder expression)
        {
            this.Visit(expression.Operator);
            this.Visit(expression.Expression);
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
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.DISTINCT);
            }
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.Identifier(expression.Column.Table.TableName, identifier));
        }

        protected virtual void VisitParameter(IParameterBuilder expression)
        {
            this.Builder.AppendFormat("{0}{1} ", this.Database.QueryFactory.Dialect.PARAMETER, expression.Name);
            if (!this.ContainsParameter(expression.Name))
            {
                this.Parameters.Add(new DatabaseQueryParameter(expression.Name, expression.Type));
            }
        }

        protected virtual void VisitFunction(IFunctionBuilder expression)
        {
            var handler = default(SqlQueryWriterDialectHandler);
            if (!Functions.TryGetValue(expression.Function, out handler))
            {
                throw new NotImplementedException();
            }
            this.Builder.AppendFormat("{0} ", handler(this));
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.OPEN_PARENTHESES);
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
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CLOSE_PARENTHESES);
        }

        protected virtual void VisitOperator(IOperatorBuilder expression)
        {
            var handler = default(SqlQueryWriterDialectHandler);
            if (!Operators.TryGetValue(expression.Operator, out handler))
            {
                throw new NotImplementedException();
            }
            this.Builder.AppendFormat("{0} ", handler(this));
        }

        protected virtual void VisitConstant(IConstantBuilder expression)
        {
            if (expression.Value == null)
            {
                this.Visit(expression.CreateOperator(QueryOperator.Null));
            }
            else
            {
                var type = expression.Value.GetType();
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
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
            this.Parameters.AddRange(query.Parameters.Except(this.Parameters));
        }

        protected virtual void VisitSequence(ISequenceBuilder expression)
        {
            var first = true;
            foreach (var element in expression.Expressions)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.LIST_DELIMITER);
                }
                this.Visit(element);
            }
        }

        protected virtual void VisitType(ITypeConfig type)
        {
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.Types.GetType(type));
        }

        protected virtual void VisitAlias(string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                return;
            }
            this.Builder.AppendFormat("{0} {1} ", this.Database.QueryFactory.Dialect.AS, this.Database.QueryFactory.Dialect.Identifier(alias));
        }

        protected virtual bool ContainsParameter(string name)
        {
            return this.Parameters.Any(parameter => string.Equals(parameter.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public delegate string SqlQueryWriterDialectHandler(SqlQueryWriter writer);

        public delegate void SqlQueryWriterVisitorHandler(SqlQueryWriter writer, IFragmentBuilder fragment);
    }

    [Flags]
    public enum RenderHints : byte
    {
        None = 0,
        FunctionArgument = 1
    }
}
