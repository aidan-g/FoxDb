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

        protected Stack<IFragmentBuilder> Context { get; private set; }

        public IFragmentBuilder Peek
        {
            get
            {
                return this.Context.Peek();
            }
        }

        public T Push<T>(T builder) where T : IFragmentBuilder
        {
            this.Context.Push(builder);
            return builder;
        }

        public IFragmentBuilder Pop()
        {
            return this.Context.Pop();
        }

        IReadOnlyCollection<IFragmentBuilder> ISQLiteQueryWriter.Context
        {
            get
            {
                return this.Context;
            }
        }

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
                this.Context = new Stack<IFragmentBuilder>((parent as ISQLiteQueryWriter).Context);
            }
            else
            {
                this.Context = new Stack<IFragmentBuilder>();
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

        public T GetContext<T>() where T : IFragmentBuilder
        {
            return this.Context.OfType<T>().FirstOrDefault();
        }

        public T Write<T>(T fragment) where T : IFragmentBuilder
        {
            this.Context.Push(fragment);
            try
            {
                return this.OnWrite(fragment);
            }
            finally
            {
                this.Context.Pop();
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
            this.Context.Push(expression);
            try
            {
                handler(this, expression);
            }
            finally
            {
                this.Context.Pop();
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
                this.Visit(expression.Expressions);
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
}
