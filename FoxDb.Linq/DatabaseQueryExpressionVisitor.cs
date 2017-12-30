using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public class DatabaseQueryExpressionVisitor<T> : ExpressionVisitor
    {
        protected readonly IDictionary<ExpressionType, QueryOperator> Operators = new Dictionary<ExpressionType, QueryOperator>()
        {
            { ExpressionType.Equal, QueryOperator.Equal },
            { ExpressionType.NotEqual, QueryOperator.NotEqual },
            { ExpressionType.And, QueryOperator.And },
            { ExpressionType.AndAlso, QueryOperator.AndAlso },
            { ExpressionType.Or, QueryOperator.Or },
            { ExpressionType.OrElse, QueryOperator.OrElse },
        };

        private DatabaseQueryExpressionVisitor()
        {
            this.Constants = new Dictionary<string, object>();
            this.Targets = new Stack<IFragmentTarget>();
        }

        public DatabaseQueryExpressionVisitor(IDatabase database) : this()
        {
            this.Database = database;
            this.Begin();
        }

        protected IDictionary<string, object> Constants { get; private set; }

        protected Stack<IFragmentTarget> Targets { get; private set; }

        public IDatabase Database { get; private set; }

        public IQueryGraphBuilder Builder { get; private set; }

        public ITableConfig Table
        {
            get
            {
                return this.Database.Config.Table<T>();
            }
        }

        public IDatabaseQuery Query
        {
            get
            {
                return this.Database.QueryFactory.Create(this.Builder.Build());
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

        protected virtual void Begin()
        {
            this.Builder = this.Database.QueryFactory.Build();
            this.Builder.Select.AddColumns(this.Table.Columns);
            this.Builder.From.AddTable(this.Table);
        }

        protected virtual IFragmentTarget Target
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

        protected virtual IFragmentTarget Push(IFragmentTarget target)
        {
            this.Targets.Push(target);
            return target;
        }

        protected virtual IFragmentTarget Pop()
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

        protected virtual void Visit(ExpressionType nodeType)
        {
            var value = default(QueryOperator);
            if (!this.Operators.TryGetValue(nodeType, out value))
            {
                throw new NotImplementedException();
            }
            this.Target.Write(this.Target.GetOperator(value));
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable) && node.Method.Name == "Where")
            {
                this.Push(this.Builder.Where);
                this.Visit(node.Arguments[0]);
                var lambda = this.GetLambda(node.Arguments[1]);
                this.Visit(lambda.Body);
                return node;
            }
            throw new NotImplementedException();
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
            var fragment = this.Push(this.Target.GetFragment<IBinaryExpressionBuilder>());
            this.Visit(node.Left);
            this.Visit(node.NodeType);
            this.Visit(node.Right);
            this.Pop();
            this.Target.Write(fragment);
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
                this.Target.Write(this.Target.GetColumn(this.Table.Column(property)));
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
                this.Target.Write(this.Target.GetOperator(QueryOperator.Null));
            }
            else
            {
                var name = string.Format("parameter{0}", this.Constants.Count);
                this.Target.Write(this.Target.GetParameter(name));
                this.Constants[name] = node.Value;
            }
            return base.VisitConstant(node);
        }
    }
}
