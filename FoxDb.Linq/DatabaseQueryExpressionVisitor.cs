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
        private DatabaseQueryExpressionVisitor()
        {
            this.Constants = new Dictionary<string, object>();
        }

        public DatabaseQueryExpressionVisitor(IDatabase database) : this()
        {
            this.Database = database;
            this.Table = database.Config.Table<T>();
        }

        public IDatabase Database { get; private set; }

        public ITableConfig<T> Table { get; private set; }

        public IDatabaseQueryComposer Composer { get; private set; }

        public IDatabaseQuery Query
        {
            get
            {
                return this.Composer.Query;
            }
        }

        protected IDictionary<string, object> Constants { get; private set; }

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

        protected virtual void BeginQuery()
        {
            this.Composer = this.Database.QueryFactory.Compose();
            this.Composer.Select();
            this.Composer.Table(this.Table);
            this.Composer.IdentifierDelimiter();
            this.Composer.Star();
            this.Composer.From();
            this.Composer.Table(this.Table);
            this.Composer.Where();
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

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable) && node.Method.Name == "Where")
            {
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
            return node;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            this.Composer.OpenParentheses();
            this.Visit(node.Left);
            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    this.Composer.Equal();
                    break;
                case ExpressionType.NotEqual:
                    this.Composer.NotEqual();
                    break;
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    this.Composer.And();
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    this.Composer.Or();
                    break;
                default:
                    throw new NotImplementedException();
            }
            this.Visit(node.Right);
            this.Composer.CloseParentheses();
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
                this.Composer.Column(this.Table.Column(property));
                return node;
            }
            throw new NotImplementedException();
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value is IQueryable)
            {
                this.BeginQuery();
            }
            else if (node.Value == null)
            {
                this.Composer.Null();
            }
            else
            {
                var name = string.Format("parameter{0}", this.Constants.Count);
                this.Composer.Parameter(name);
                this.Constants[name] = node.Value;
            }
            return base.VisitConstant(node);
        }
    }
}
