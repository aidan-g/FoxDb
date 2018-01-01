using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public abstract class QueryFragmentVisitor : ExpressionVisitor
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

        protected QueryFragmentVisitor(IDatabaseQueryableTarget target, Type elementType)
        {
            this.Target = target;
            this.ElementType = elementType;
        }

        public IDatabaseQueryableTarget Target { get; private set; }

        public Type ElementType { get; private set; }

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
            var @operator = default(QueryOperator);
            if (!this.Operators.TryGetValue(nodeType, out @operator))
            {
                throw new NotImplementedException();
            }
            this.Visit(@operator);
        }

        protected virtual void Visit(QueryOperator @operator)
        {
            this.Target.Peek.Write(this.Target.Peek.GetOperator(@operator));
        }

        protected virtual void Visit(PropertyInfo property)
        {
            var table = this.Target.Database.Config.Table(this.ElementType);
            this.Visit(table.Column(property));
        }

        protected virtual void Visit(IColumnConfig column)
        {
            this.Target.Peek.Write(this.Target.Peek.GetColumn(column));
        }

        protected virtual void Visit(string name, object value)
        {
            this.Target.Peek.Write(this.Target.Peek.GetParameter(name));
            this.Target.Constants[name] = value;
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
            var fragment = this.Target.Push(this.Target.Peek.GetFragment<IBinaryExpressionBuilder>());
            this.Visit(node.Left);
            this.Visit(node.NodeType);
            this.Visit(node.Right);
            this.Target.Pop();
            this.Target.Peek.Write(fragment);
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
                var name = string.Format("parameter{0}", this.Target.Constants.Count);
                this.Visit(name, node.Value);
            }
            return base.VisitConstant(node);
        }
    }
}
