using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public class EnumerableVisitor : ExpressionVisitor
    {
        public static readonly IDictionary<string, MethodVisitorHandler> MethodHandlers = GetMethodHandlers();

        public static readonly IDictionary<ExpressionType, UnaryVisitorHandler> UnaryHandlers = GetUnaryHandlers();

        public static readonly IDictionary<ExpressionType, QueryOperator> Operators = GetOperators();

        protected static IDictionary<string, MethodVisitorHandler> GetMethodHandlers()
        {
            var handlers = new Dictionary<string, MethodVisitorHandler>(StringComparer.OrdinalIgnoreCase)
            {
                { "Count", (visitor, node) => visitor.VisitCount(node) },
                { "Any", (visitor, node) => visitor.VisitAny(node) },
                { "First", (visitor, node) => visitor.VisitFirst(node) },
                { "FirstOrDefault", (visitor, node) => visitor.VisitFirst(node) },
                { "Select", (visitor, node) => visitor.VisitSelect(node) },
                { "Where", (visitor, node) => visitor.VisitWhere(node) },
                { "Except", (visitor, node) => visitor.VisitExcept(node) },
                { "OrderBy", (visitor, node) => visitor.VisitOrderBy(node) },
                { "OrderByDescending", (visitor, node) => visitor.VisitOrderByDescending(node) }
            };
            return handlers;
        }

        protected static IDictionary<ExpressionType, UnaryVisitorHandler> GetUnaryHandlers()
        {
            return new Dictionary<ExpressionType, UnaryVisitorHandler>()
            {
                { ExpressionType.Convert, (visitor, node) => visitor.VisitConvert(node) },
                { ExpressionType.Quote, (visitor, node) => visitor.VisitQuote(node) }
            };
        }

        protected static IDictionary<ExpressionType, QueryOperator> GetOperators()
        {
            return new Dictionary<ExpressionType, QueryOperator>()
            {
                //Logical.
                { ExpressionType.Not,  QueryOperator.Not },
                { ExpressionType.Equal, QueryOperator.Equal },
                { ExpressionType.NotEqual, QueryOperator.NotEqual },
                { ExpressionType.LessThan, QueryOperator.Less },
                { ExpressionType.LessThanOrEqual, QueryOperator.LessOrEqual },
                { ExpressionType.GreaterThan, QueryOperator.Greater },
                { ExpressionType.GreaterThanOrEqual, QueryOperator.GreaterOrEqual },
                { ExpressionType.And, QueryOperator.And },
                { ExpressionType.AndAlso, QueryOperator.AndAlso },
                { ExpressionType.Or, QueryOperator.Or },
                { ExpressionType.OrElse, QueryOperator.OrElse },
                //Mathematical.
                { ExpressionType.Add, QueryOperator.Add }
            };
        }

        private EnumerableVisitor()
        {
            this.Id = Unique.New;
            this.Constants = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this.Targets = new Stack<IFragmentTarget>();
        }

        public EnumerableVisitor(IDatabaseSetQuery provider, IDatabase database, IQueryGraphBuilder query, Type elementType)
            : this()
        {
            this.Provider = provider;
            this.Database = database;
            this.Query = query;
            this.ElementType = elementType;
        }

        public string Id { get; private set; }

        public IDictionary<string, object> Constants { get; private set; }

        protected Stack<IFragmentTarget> Targets { get; private set; }

        public IDatabaseSetQuery Provider { get; private set; }

        public IDatabase Database { get; private set; }

        public Type ElementType { get; private set; }

        public Type EnumerableType
        {
            get
            {
                return typeof(IEnumerable<>).MakeGenericType(this.ElementType);
            }
        }

        public Type QueryableType
        {
            get
            {
                return typeof(IQueryable<>).MakeGenericType(this.ElementType);
            }
        }

        public OrderByDirection Direction { get; private set; }

        public IQueryGraphBuilder Query { get; private set; }

        public ITableConfig Table
        {
            get
            {
                return this.Database.Config.GetTable(TableConfig.By(this.ElementType, Defaults.Table.Flags));
            }
        }

        public DatabaseParameterHandler Parameters
        {
            get
            {
                return new DatabaseParameterHandler(parameters =>
                {
                    foreach (var key in this.Constants.Keys)
                    {
                        if (parameters.Contains(key))
                        {
                            parameters[key] = this.Constants[key];
                        }
                    }
                });
            }
        }

        public bool TryPeek()
        {
            var target = default(IFragmentTarget);
            return this.TryPeek(out target);
        }

        public bool TryPeek(out IFragmentTarget target)
        {
            if (this.Targets.Count == 0)
            {
                target = default(IFragmentTarget);
                return false;
            }
            target = this.Targets.Peek();
            return true;
        }

        public IFragmentTarget Peek
        {
            get
            {
                if (this.Targets.Count == 0)
                {
                    throw new InvalidOperationException("No target to write fragment to.");
                }
                return this.Targets.Peek();
            }
        }

        public T Push<T>(T target) where T : IFragmentTarget
        {
            this.Targets.Push(target);
            return target;
        }

        public IFragmentTarget Pop(bool importConstants = true)
        {
            var target = this.Targets.Pop();
            if (importConstants)
            {
                this.ImportConstants(target);
            }
            return target;
        }

        protected virtual void ImportConstants(IFragmentTarget target)
        {
            foreach (var key in target.Constants.Keys)
            {
                var value = target.Constants[key];
                if (value != null)
                {
                    var type = value.GetType();
                    if (!type.IsScalar())
                    {
                        throw new InvalidOperationException(string.Format("Constant with name \"{0}\" has invalid type \"{1}\".", key, type.FullName));
                    }
                }
                this.Constants[key] = target.Constants[key];
            }
        }

        protected virtual bool TryGetTable(MemberInfo member, out ITableConfig result)
        {
            if (member.DeclaringType != this.ElementType)
            {
                result = default(ITableConfig);
                return false;
            }
            var type = this.GetMemberType(member);
            if (type.IsGenericType)
            {
                result = default(ITableConfig);
                return false;
            }
            result = this.Database.Config.GetTable(TableConfig.By(type, Defaults.Table.Flags));
            return result != null;
        }

        protected virtual bool TryGetRelation(MemberInfo member, out IRelationConfig result)
        {
            if (member.DeclaringType != this.ElementType)
            {
                result = default(IRelationConfig);
                return false;
            }
            var type = this.GetMemberType(member);
            if (type.IsGenericType)
            {
                type = type.GetGenericArguments()[0];
            }
            var table = this.Database.Config.GetTable(TableConfig.By(member.DeclaringType, Defaults.Table.Flags));
            if (table != null)
            {
                foreach (var relation in table.Relations)
                {
                    if (relation.RelationType == type)
                    {
                        result = relation;
                        return true;
                    }
                }
            }
            result = default(IRelationConfig);
            return false;
        }

        protected virtual bool TryGetColumn(MemberInfo member, out IColumnConfig result)
        {
            var property = member as PropertyInfo;
            if (property == null)
            {
                result = default(IColumnConfig);
                return false;
            }
            var table = this.Database.Config.GetTable(TableConfig.By(member.DeclaringType, Defaults.Table.Flags));
            if (table == null)
            {
                result = default(IColumnConfig);
                return false;
            }
            result = table.GetColumn(ColumnConfig.By(property, Defaults.Column.Flags));
            return result != null;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var handler = default(MethodVisitorHandler);
            if (!MethodHandlers.TryGetValue(node.Method.Name, out handler))
            {
                this.VisitUnsupportedMethodCall(node);
                return node;
            }
            handler(this, node);
            return node;
        }

        protected virtual void VisitUnsupportedMethodCall(MethodCallExpression node)
        {
            try
            {
                var lambda = Expression.Lambda(node).Compile();
                var value = lambda.DynamicInvoke();
                this.VisitParameter(value);
            }
            catch (Exception e)
            {
                throw new NotImplementedException(string.Format("The method \"{0}\" of type \"{0}\" is unsupported and could not be evaluated.", node.Method.Name, node.Type.FullName), e);
            }
        }

        protected virtual void VisitFirst(MethodCallExpression node)
        {
            if (this.Table.TableType == node.Type)
            {
                var filter = this.Query.Source.GetTable(this.Table).Filter;
                filter.Limit = 1;
                this.Push(filter);
                try
                {
                    foreach (var argument in node.Arguments)
                    {
                        this.Visit(argument);
                    }
                }
                finally
                {
                    this.Pop();
                }
            }
            else
            {
                this.VisitUnsupportedMethodCall(node);
            }
        }

        protected virtual void VisitCount(MethodCallExpression node)
        {
            //Count is not implemented here, LINQ will use the .Count property of IDatabaseSet.
            //We just need to process the predicate (if one was supplied).
            this.VisitWhere(node);
        }

        protected virtual void VisitAny(MethodCallExpression node)
        {
            var relation = default(IRelationBuilder);
            if (this.TryCapture<IRelationBuilder>(null, node.Arguments[0], out relation))
            {
                this.VisitAny(node, relation.Relation);
            }
            else
            {
                this.VisitWhere(node);
            }
        }

        protected virtual void VisitAny(MethodCallExpression node, IRelationConfig relation)
        {
            var target = default(IFragmentTarget);
            var columns = relation.Expression.GetColumnMap();
            if (!this.TryPeek(out target))
            {
                target = this.Query.Filter;
            }
            target.Write(target.CreateFunction(QueryFunction.Exists).With(function =>
            {
                var builder = this.Database.QueryFactory.Build();
                builder.Output.AddOperator(QueryOperator.Star);
                builder.Source.AddTable(relation.LeftTable.Extern());
                builder.Source.AddTable(relation.RightTable);
                builder.RelationManager.AddRelation(relation);
                function.AddArgument(function.CreateSubQuery(builder));
                this.Push(builder.Filter);
            }));
            try
            {
                for (var a = 1; a < node.Arguments.Count; a++)
                {
                    this.Visit(node.Arguments[a]);
                }
            }
            finally
            {
                this.Pop();
            }
        }

        protected virtual void VisitSelect(MethodCallExpression node)
        {
            this.Query.Output.Expressions.Clear();
            this.Visit(node.Arguments[0]);
            this.Push(this.Query.Output);
            try
            {
                for (var a = 1; a < node.Arguments.Count; a++)
                {
                    this.Visit(node.Arguments[a]);
                }
            }
            finally
            {
                this.Pop();
            }
        }

        protected virtual void VisitWhere(MethodCallExpression node)
        {
            this.Visit(node.Arguments[0]);
            switch (node.Arguments.Count)
            {
                case 1:
                    break;
                case 2:
                    var pop = false;
                    if (!this.TryPeek())
                    {
                        this.Push(this.Query.Filter);
                        pop = true;
                    }
                    try
                    {
                        this.Visit(node.Arguments[1]);
                    }
                    finally
                    {
                        if (pop)
                        {
                            this.Pop();
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual void VisitExcept(MethodCallExpression node)
        {
            this.Visit(node.Arguments[0]);
            switch (node.Arguments.Count)
            {
                case 1:
                    break;
                case 2:
                    var pop = false;
                    if (!this.TryPeek())
                    {
                        this.Push(this.Query.Filter);
                        pop = true;
                    }
                    try
                    {
                        this.Peek.Write(
                            this.Peek.CreateUnary(
                                QueryOperator.Not,
                                this.Push(this.Peek.CreateBinary(
                                    this.Peek.CreateColumn(this.Table.PrimaryKey),
                                    QueryOperator.In,
                                    //Leave the right expression null, we will write to this later.
                                    null
                                )).With(binary =>
                                {
                                    try
                                    {
                                        switch (node.Arguments[1].NodeType)
                                        {
                                            case ExpressionType.Constant:
                                                this.Visit(node.Arguments[1]);
                                                break;
                                            default:
                                                throw new NotImplementedException();
                                        }
                                    }
                                    finally
                                    {
                                        this.Pop();
                                    }
                                })
                            )
                        );
                    }
                    finally
                    {
                        if (pop)
                        {
                            this.Pop();
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual void VisitOrderBy(MethodCallExpression node)
        {
            this.Query.Sort.Expressions.Clear();
            this.Direction = OrderByDirection.None;
            this.Visit(node.Arguments[0]);
            this.Push(this.Query.Sort);
            try
            {
                for (var a = 1; a < node.Arguments.Count; a++)
                {
                    this.Visit(node.Arguments[a]);
                }
            }
            finally
            {
                this.Pop();
            }
        }

        protected virtual void VisitOrderByDescending(MethodCallExpression node)
        {
            this.Query.Sort.Expressions.Clear();
            this.Direction = OrderByDirection.Descending;
            this.Visit(node.Arguments[0]);
            this.Push(this.Query.Sort);
            try
            {
                for (var a = 1; a < node.Arguments.Count; a++)
                {
                    this.Visit(node.Arguments[a]);
                }
            }
            finally
            {
                this.Pop();
            }
        }

        protected virtual void VisitQueryable(IQueryable queryable)
        {
            //Nothing to do.
        }

        protected virtual void VisitEnumerable(IEnumerable enumerable)
        {
            this.Peek.Write(this.Push(this.Peek.CreateSequence()));
            try
            {
                var success = false;
                foreach (var element in enumerable)
                {
                    this.VisitParameter(this.Table.PrimaryKey.Getter(element));
                    success = true;
                }
                if (!success)
                {
                    this.VisitParameter(this.Table.PrimaryKey.DefaultValue);
                }
            }
            finally
            {
                this.Pop();
            }
        }

        protected virtual IOperatorBuilder VisitOperator(ExpressionType nodeType)
        {
            var @operator = default(QueryOperator);
            if (!Operators.TryGetValue(nodeType, out @operator))
            {
                throw new NotImplementedException();
            }
            return this.VisitOperator(@operator);
        }

        protected virtual IOperatorBuilder VisitOperator(QueryOperator @operator)
        {
            return this.Peek.Write(this.Peek.CreateOperator(@operator));
        }

        protected virtual bool TryVisitMember(MemberInfo member, Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Parameter:
                    break;
                default:
                    return false;
            }
            var table = default(ITableConfig);
            var relation = default(IRelationConfig);
            var column = default(IColumnConfig);
            if (this.TryGetTable(member, out table))
            {
                this.VisitTable(table);
                return true;
            }
            else if (this.TryGetRelation(member, out relation))
            {
                this.VisitRelation(relation);
                return true;
            }
            else if (this.TryGetColumn(member, out column))
            {
                this.VisitColumn(column);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual ITableBuilder VisitTable(ITableConfig table)
        {
            return this.Peek.Write(this.Peek.CreateTable(table));
        }

        protected virtual IRelationBuilder VisitRelation(IRelationConfig relation)
        {
            return this.Peek.Write(this.Peek.CreateRelation(relation));
        }

        protected virtual IColumnBuilder VisitColumn(IColumnConfig column)
        {
            return this.Peek.Write(this.Peek.CreateColumn(column).With(builder => builder.Direction = this.Direction));
        }

        protected virtual IParameterBuilder VisitParameter(object value)
        {
            var name = this.GetParameterName();
            var parameter = this.Peek.Write(this.Peek.CreateParameter(name));
            this.Peek.Constants[name] = value;
            return parameter;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            var handler = default(UnaryVisitorHandler);
            if (!UnaryHandlers.TryGetValue(node.NodeType, out handler))
            {
                this.VisitUnsupportedUnary(node);
                return node;
            }
            handler(this, node);
            return node;
        }

        protected virtual void VisitUnsupportedUnary(UnaryExpression node)
        {
            var fragment = this.Push(this.Peek.Fragment<IUnaryExpressionBuilder>());
            try
            {
                this.VisitOperator(node.NodeType);
                this.Visit(node.Operand);
            }
            finally
            {
                this.Pop();
            }
            this.Peek.Write(fragment);
        }

        protected virtual void VisitConvert(UnaryExpression node)
        {
            if (node.Operand != null)
            {
                this.Visit(node.Operand);
            }
        }

        protected virtual void VisitQuote(UnaryExpression node)
        {
            if (node.Operand != null)
            {
                this.Visit(node.Operand);
            }
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var fragment = this.Push(this.Peek.Fragment<IBinaryExpressionBuilder>());
            try
            {
                this.Visit(node.Left);
                this.VisitOperator(node.NodeType);
                this.Visit(node.Right);
            }
            finally
            {
                this.Pop();
            }
            if (fragment.Right == null)
            {
                //Sometimes a binary expression evaluates to a unary expression, such as ...Any() == true.
                //In this case we can ignore everything other than the left expression.
                this.Peek.Write(fragment.Left);
            }
            else
            {
                this.Peek.Write(fragment);
            }
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (this.TryVisitMember(node.Member, node.Expression))
            {
                return node;
            }
            else if (this.TryUnwrapConstant(node.Member, node.Expression))
            {
                return node;
            }
            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value == null)
            {
                this.VisitOperator(QueryOperator.Null);
            }
            else
            {
                var type = node.Value.GetType();
                if (this.QueryableType.IsAssignableFrom(type))
                {
                    this.VisitQueryable(node.Value as IQueryable);
                }
                else if (this.EnumerableType.IsAssignableFrom(type))
                {
                    this.VisitEnumerable(node.Value as IEnumerable);
                }
                else if (this.TryPeek())
                {
                    this.VisitParameter(node.Value);
                }
            }
            return base.VisitConstant(node);
        }

        protected virtual bool TryUnwrapConstant(MemberInfo member, Expression node)
        {
            var context = default(CaptureFragmentContext);
            this.Capture<IParameterBuilder>(null, node, out context);
            foreach (var key in context.Constants.Keys)
            {
                var value = context.Constants[key];
                if (value == null)
                {
                    continue;
                }
                if (!member.DeclaringType.IsAssignableFrom(value.GetType()))
                {
                    continue;
                }
                this.VisitParameter(this.GetMemberValue(member, value));
                return true;
            }
            return false;
        }

        protected virtual Type GetMemberType(MemberInfo member)
        {
            if (member is FieldInfo)
            {
                return (member as FieldInfo).FieldType;
            }
            else if (member is PropertyInfo)
            {
                return (member as PropertyInfo).PropertyType;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected virtual object GetMemberValue(MemberInfo member, object value)
        {
            if (member is FieldInfo)
            {
                return (member as FieldInfo).GetValue(value);
            }
            else if (member is PropertyInfo)
            {
                return (member as PropertyInfo).GetValue(value);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected virtual string GetExtentName()
        {
            return string.Format("extent_{0}_{1}", this.Id, Unique.New);
        }

        protected virtual string GetParameterName()
        {
            var count = this.Constants.Count;
            var target = this.Targets.Peek();
            if (target != null)
            {
                count += target.Constants.Count;
            }
            return string.Format("parameter_{0}_{1}", this.Id, count);
        }

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

            public override string DebugView
            {
                get
                {
                    return string.Format("{{{0}}}", string.Join(", ", this.Context.Expressions.Select(expression => expression.DebugView)));
                }
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

        public delegate void MethodVisitorHandler(EnumerableVisitor visitor, MethodCallExpression node);

        public delegate void UnaryVisitorHandler(EnumerableVisitor visitor, UnaryExpression node);
    }
}
