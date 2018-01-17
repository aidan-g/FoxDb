using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public class DatabaseQueryableVisitor : ExpressionVisitor
    {
        protected virtual IDictionary<string, MethodVisitorHandler> MethodHandlers { get; private set; }

        protected virtual IDictionary<string, MethodVisitorHandler> GetMethodHandlers()
        {
            return new Dictionary<string, MethodVisitorHandler>()
            {
                //Scalar methods.
                { "First", this.VisitFirst },
                { "FirstOrDefault", this.VisitFirst },
                { "Count", this.VisitCount },
                //Enumerable methods.
                { "Any", this.VisitAny },
                { "Where", this.VisitWhere },
                { "OrderBy", this.VisitOrderBy },
                { "OrderByDescending", this.VisitOrderByDescending }
            };
        }

        protected virtual IDictionary<ExpressionType, UnaryVisitorHandler> UnaryHandlers { get; private set; }

        protected virtual IDictionary<ExpressionType, UnaryVisitorHandler> GetUnaryHandlers()
        {
            return new Dictionary<ExpressionType, UnaryVisitorHandler>()
            {
                { ExpressionType.Convert, this.VisitConvert },
                { ExpressionType.Quote, this.VisitQuote }
            };
        }

        protected readonly IDictionary<ExpressionType, QueryOperator> Operators = new Dictionary<ExpressionType, QueryOperator>()
        {
            { ExpressionType.Equal, QueryOperator.Equal },
            { ExpressionType.NotEqual, QueryOperator.NotEqual },
            { ExpressionType.LessThan, QueryOperator.Less },
            { ExpressionType.GreaterThan, QueryOperator.Greater },
            { ExpressionType.And, QueryOperator.And },
            { ExpressionType.AndAlso, QueryOperator.AndAlso },
            { ExpressionType.Or, QueryOperator.Or },
            { ExpressionType.OrElse, QueryOperator.OrElse },
        };

        private DatabaseQueryableVisitor()
        {
            this.MethodHandlers = this.GetMethodHandlers();
            this.UnaryHandlers = this.GetUnaryHandlers();
            this.Constants = new Dictionary<string, object>();
            this.Targets = new Stack<IFragmentTarget>();
        }

        public DatabaseQueryableVisitor(IDatabase database, IQueryGraphBuilder query, Type elementType) : this()
        {
            this.Database = database;
            this.Query = query;
            this.ElementType = elementType;
        }

        public IDictionary<string, object> Constants { get; private set; }

        protected Stack<IFragmentTarget> Targets { get; private set; }

        public IDatabase Database { get; private set; }

        public Type ElementType { get; private set; }

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

        protected virtual LambdaExpression GetLambda(Expression node)
        {
            while (node != null && !(node is LambdaExpression))
            {
                if (node is UnaryExpression)
                {
                    node = (node as UnaryExpression).Operand;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            return node as LambdaExpression;
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
            if (!this.MethodHandlers.TryGetValue(node.Method.Name, out handler))
            {
                this.VisitUnsupportedMethodCall(node);
                return node;
            }
            handler(node);
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
            if (typeof(IQueryable).IsAssignableFrom(node.Arguments[0].Type))
            {
                //Count is not implemented here, LINQ will use the .Count property of IDatabaseSet.
                this.Push(this.Query.Filter);
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

        protected virtual void VisitAny(MethodCallExpression node)
        {
            var relation = this.Capture<IRelationBuilder>(node.Arguments[0]).Relation;
            this.Query.Filter.AddFunction(this.Query.Filter.CreateFunction(QueryFunction.Exists).With(function =>
            {
                var query = this.Database.QueryFactory.Build();
                query.Output.AddOperator(QueryOperator.Star);
                query.Source.AddTable(relation.RightTable);
                switch (relation.Flags.GetMultiplicity())
                {
                    case RelationFlags.OneToMany:
                        query.Filter.AddColumn(relation.RightTable.ForeignKey, relation.LeftTable.PrimaryKey);
                        break;
                    case RelationFlags.ManyToMany:
                        query.Source.AddRelation(relation.Invert());
                        query.Filter.AddColumn(relation.LeftColumn, relation.LeftTable.PrimaryKey);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                function.AddArgument(function.CreateSubQuery(query));
                this.Push(query.Filter);
            }));
            try
            {
                var lambda = this.GetLambda(node.Arguments[1]);
                this.Visit(lambda.Body);
            }
            finally
            {
                this.Pop();
            }
        }

        protected virtual void VisitWhere(MethodCallExpression node)
        {
            this.Visit(node.Arguments[0]);
            this.Push(this.Query.Filter);
            try
            {
                var lambda = this.GetLambda(node.Arguments[1]);
                this.Visit(lambda.Body);
            }
            finally
            {
                this.Pop();
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
                var lambda = this.GetLambda(node.Arguments[1]);
                this.Visit(lambda.Body);
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
                var lambda = this.GetLambda(node.Arguments[1]);
                this.Visit(lambda.Body);
            }
            finally
            {
                this.Pop();
            }
        }

        protected virtual IOperatorBuilder VisitOperator(ExpressionType nodeType)
        {
            var @operator = default(QueryOperator);
            if (!this.Operators.TryGetValue(nodeType, out @operator))
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
            if (!this.UnaryHandlers.TryGetValue(node.NodeType, out handler))
            {
                throw new NotImplementedException();
            }
            handler(node);
            return node;
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
            var fragment = this.Push(this.Peek.CreateFragment<IBinaryExpressionBuilder>());
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
            //if (fragment.Left == null || fragment.Operator == null || fragment.Right == null)
            //{
            //    throw new InvalidOperationException(string.Format("Binary expression is invalid: Left = \"{0}\" Operator = \"{1}\" Right = \"{2}\".", fragment.Left, fragment.Operator, fragment.Right));
            //}
            if (fragment.Right == null)
            {
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
            if (node.Value is IQueryable)
            {
                //Nothing to do.
            }
            else if (node.Value == null)
            {
                this.VisitOperator(QueryOperator.Null);
            }
            else
            {
                this.VisitParameter(node.Value);
            }
            return base.VisitConstant(node);
        }

        protected virtual bool TryUnwrapConstant(MemberInfo member, Expression node)
        {
            var constants = default(IDictionary<string, object>);
            this.Capture<IParameterBuilder>(node, out constants);
            foreach (var key in constants.Keys)
            {
                var value = constants[key];
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

        protected virtual string GetParameterName()
        {
            var count = this.Constants.Count;
            var target = this.Targets.Peek();
            if (target != null)
            {
                count += target.Constants.Count;
            }
            return string.Format("parameter{0}", count);
        }

        protected virtual T Capture<T>(Expression node) where T : IFragmentBuilder
        {
            var constants = default(IDictionary<string, object>);
            var expression = this.Capture<T>(node, out constants);
            if (constants.Any())
            {
                throw new InvalidOperationException("Capture resulted in unhandled constants.");
            }
            return expression;
        }

        protected virtual T Capture<T>(Expression node, out IDictionary<string, object> constants) where T : IFragmentBuilder
        {
            var capture = new CaptureFragmentTarget();
            this.Push(capture);
            try
            {
                this.Visit(node);
            }
            finally
            {
                this.Pop(false);
            }
            foreach (var expression in capture.Expressions)
            {
                if (expression is T)
                {
                    constants = capture.Constants;
                    return (T)expression;
                }
            }
            throw new InvalidOperationException(string.Format("Failed to capture fragment of type \"{0}\".", typeof(T).FullName));
        }

        protected class CaptureFragmentTarget : FragmentBuilder, IFragmentTarget
        {
            public CaptureFragmentTarget()
            {
                this.Expressions = new List<IFragmentBuilder>();
                this.Constants = new Dictionary<string, object>();
            }

            public override FragmentType FragmentType
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public ICollection<IFragmentBuilder> Expressions { get; private set; }

            public IDictionary<string, object> Constants { get; private set; }

            public T Write<T>(T fragment) where T : IFragmentBuilder
            {
                this.Expressions.Add(fragment);
                return fragment;
            }
        }

        public delegate void MethodVisitorHandler(MethodCallExpression node);

        public delegate void UnaryVisitorHandler(UnaryExpression node);
    }
}
