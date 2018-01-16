using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public class DatabaseQueryableExpressionVisitor : ExpressionVisitor
    {
        protected virtual IDictionary<string, DatabaseQueryableExpressionVisitorHandler> Handlers { get; private set; }

        protected virtual IDictionary<string, DatabaseQueryableExpressionVisitorHandler> GetHandlers()
        {
            return new Dictionary<string, DatabaseQueryableExpressionVisitorHandler>()
            {
                //Scalar methods.
                { "First", this.VisitFirst },
                //Enumerable methods.
                { "Any", this.VisitAny },
                { "Where", this.VisitWhere },
                { "OrderBy", this.VisitOrderBy },
                { "OrderByDescending", this.VisitOrderByDescending }
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

        private DatabaseQueryableExpressionVisitor()
        {
            this.Handlers = this.GetHandlers();
            this.Constants = new Dictionary<string, object>();
            this.Targets = new Stack<IFragmentTarget>();
        }

        public DatabaseQueryableExpressionVisitor(IDatabase database, IQueryGraphBuilder query, Type elementType) : this()
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

        public IFragmentTarget Push(IFragmentTarget target)
        {
            this.Targets.Push(target);
            return target;
        }

        public IFragmentTarget Pop()
        {
            return this.Targets.Pop();
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

        protected virtual ConstantExpression GetConstant(Expression node)
        {
            while (node != null && !(node is ConstantExpression))
            {
                if (node is MemberExpression)
                {
                    var member = node as MemberExpression;
                    var target = this.GetConstant(member.Expression);
                    var value = default(object);
                    if (member.Member is FieldInfo)
                    {
                        var field = member.Member as FieldInfo;
                        value = field.GetValue(target.Value);
                    }
                    else if (member.Member is PropertyInfo)
                    {
                        var property = member.Member as PropertyInfo;
                        value = property.GetValue(target.Value);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    return Expression.Constant(value);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            return node as ConstantExpression;
        }

        protected virtual bool TryGetRelation(PropertyInfo property, out IRelationConfig result)
        {
            if (property.DeclaringType != this.ElementType)
            {
                result = default(IRelationConfig);
                return false;
            }
            var type = property.PropertyType;
            if (type.IsGenericType)
            {
                type = type.GetGenericArguments()[0];
            }
            var table = this.Database.Config.Table(property.DeclaringType);
            foreach (var relation in table.Relations)
            {
                if (relation.RelationType == type)
                {
                    result = relation;
                    return true;
                }
            }
            result = default(IRelationConfig);
            return false;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var handler = default(DatabaseQueryableExpressionVisitorHandler);
            if (!this.Handlers.TryGetValue(node.Method.Name, out handler))
            {
                throw new NotImplementedException();
            }
            handler(node);
            return node;
        }

        protected virtual void VisitFirst(MethodCallExpression node)
        {
            this.Visit(node.Arguments[0]);
        }

        protected virtual void VisitAny(MethodCallExpression node)
        {
            var relation = this.Capture<IRelationBuilder>(node.Arguments[0]).Relation;
            this.Query.Where.AddFunction(this.Query.Where.GetFunction(QueryFunction.Exists).With(function =>
            {
                var query = this.Database.QueryFactory.Build();
                query.Select.AddOperator(QueryOperator.Star);
                query.From.AddTable(relation.RightTable);
                switch (relation.Flags.GetMultiplicity())
                {
                    case RelationFlags.OneToMany:
                        query.Where.AddColumn(relation.RightTable.ForeignKey, relation.LeftTable.PrimaryKey);
                        break;
                    case RelationFlags.ManyToMany:
                        query.From.AddRelation(relation.Invert());
                        query.Where.AddColumn(relation.LeftColumn, relation.LeftTable.PrimaryKey);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                function.AddArgument(function.GetSubQuery(query));
                this.Push(query.Where);
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
            this.Push(this.Query.Where);
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
            this.Query.OrderBy.Expressions.Clear();
            this.Direction = OrderByDirection.None;
            this.Visit(node.Arguments[0]);
            this.Push(this.Query.OrderBy);
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
            this.Query.OrderBy.Expressions.Clear();
            this.Direction = OrderByDirection.Descending;
            this.Visit(node.Arguments[0]);
            this.Push(this.Query.OrderBy);
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

        protected virtual void Visit(ExpressionType nodeType)
        {
            var @operator = default(QueryOperator);
            if (!this.Operators.TryGetValue(nodeType, out @operator))
            {
                throw new NotImplementedException();
            }
            this.Visit(@operator);
        }

        protected virtual void Visit(QueryOperator @operator)
        {
            this.Peek.Write(this.Peek.GetOperator(@operator));
        }

        protected virtual void Visit(PropertyInfo property)
        {
            var relation = default(IRelationConfig);
            if (this.TryGetRelation(property, out relation))
            {
                this.Visit(relation);
            }
            else
            {
                var table = this.Database.Config.Table(property.DeclaringType);
                this.Visit(table.Column(property));
            }
        }

        protected virtual void Visit(ITableConfig table)
        {
            this.Peek.Write(this.Peek.GetTable(table));
        }

        protected virtual void Visit(IRelationConfig relation)
        {
            this.Peek.Write(this.Peek.GetRelation(relation));
        }

        protected virtual void Visit(IColumnConfig column)
        {
            this.Peek.Write(this.Peek.GetColumn(column).With(builder => builder.Direction = this.Direction));
        }

        protected virtual void Visit(string name, object value)
        {
            this.Peek.Write(this.Peek.GetParameter(name));
            this.Constants[name] = value;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                default:
                    throw new NotImplementedException();
            }
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var fragment = this.Push(this.Peek.GetFragment<IBinaryExpressionBuilder>());
            try
            {
                this.Visit(node.Left);
                this.Visit(node.NodeType);
                this.Visit(node.Right);
            }
            finally
            {
                this.Pop();
            }
            this.Peek.Write(fragment);
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                var type = node.Expression.Type;
                var property = node.Member as PropertyInfo;
                if (property == null)
                {
                    throw new NotImplementedException();
                }
                this.Visit(property);
                return node;
            }
            else if (node.Expression != null && node.Expression.NodeType == ExpressionType.MemberAccess)
            {
                var constant = this.GetConstant(node);
                this.VisitConstant(constant);
                return node;
            }
            throw new NotImplementedException();
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value is IQueryable)
            {
                //Nothing to do.
            }
            else if (node.Value == null)
            {
                this.Visit(QueryOperator.Null);
            }
            else
            {
                var name = string.Format("parameter{0}", this.Constants.Count);
                this.Visit(name, node.Value);
            }
            return base.VisitConstant(node);
        }

        protected virtual T Capture<T>(Expression node) where T : IFragmentBuilder
        {
            var capture = new CaptureFragmentTarget();
            this.Push(capture);
            try
            {
                this.Visit(node);
            }
            finally
            {
                this.Pop();
            }
            foreach (var expression in capture.Expressions)
            {
                if (expression is T)
                {
                    return (T)expression;
                }
            }
            return default(T);
        }

        protected class CaptureFragmentTarget : FragmentBuilder, IFragmentTarget
        {
            public CaptureFragmentTarget()
            {
                this.Expressions = new List<IFragmentBuilder>();
            }

            public override FragmentType FragmentType
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public ICollection<IFragmentBuilder> Expressions { get; private set; }

            public void Write(IFragmentBuilder fragment)
            {
                this.Expressions.Add(fragment);
            }
        }
    }

    public delegate void DatabaseQueryableExpressionVisitorHandler(MethodCallExpression node);
}
