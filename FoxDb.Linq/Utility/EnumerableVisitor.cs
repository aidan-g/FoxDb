using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public partial class EnumerableVisitor : ExpressionVisitor
    {
        public static readonly IDictionary<MethodVisitorKey, MethodVisitorHandler> MethodHandlers = GetMethodHandlers();

        public static readonly IDictionary<ExpressionType, UnaryVisitorHandler> UnaryHandlers = GetUnaryHandlers();

        public static readonly IDictionary<ExpressionType, QueryOperator> Operators = GetOperators();

        protected static IDictionary<MethodVisitorKey, MethodVisitorHandler> GetMethodHandlers()
        {
            var handlers = new Dictionary<MethodVisitorKey, MethodVisitorHandler>()
            {
                //Enumerable
                { new MethodVisitorKey(typeof(Enumerable), "Count"), (visitor, node) => visitor.VisitCount(node) },
                { new MethodVisitorKey(typeof(Enumerable), "Any"), (visitor, node) => visitor.VisitAny(node) },
                { new MethodVisitorKey(typeof(Enumerable), "First"), (visitor, node) => visitor.VisitFirst(node) },
                { new MethodVisitorKey(typeof(Enumerable), "FirstOrDefault"), (visitor, node) => visitor.VisitFirst(node) },
                { new MethodVisitorKey(typeof(Enumerable), "Select"), (visitor, node) => visitor.VisitSelect(node) },
                { new MethodVisitorKey(typeof(Enumerable), "Where"), (visitor, node) => visitor.VisitWhere(node) },
                { new MethodVisitorKey(typeof(Enumerable), "Except"), (visitor, node) => visitor.VisitExcept(node) },
                { new MethodVisitorKey(typeof(Enumerable), "OrderBy"), (visitor, node) => visitor.VisitOrderBy(node) },
                { new MethodVisitorKey(typeof(Enumerable), "OrderByDescending"), (visitor, node) => visitor.VisitOrderByDescending(node) },
                { new MethodVisitorKey(typeof(Enumerable), "Contains"), (visitor, node) => visitor.VisitContains(node) },
                { new MethodVisitorKey(typeof(Enumerable), "Skip"), (visitor, node) => visitor.VisitSkip(node) },
                { new MethodVisitorKey(typeof(Enumerable), "Take"), (visitor, node) => visitor.VisitTake(node) },
                //Queryable
                { new MethodVisitorKey(typeof(Queryable), "Count"), (visitor, node) => visitor.VisitCount(node) },
                { new MethodVisitorKey(typeof(Queryable), "Any"), (visitor, node) => visitor.VisitAny(node) },
                { new MethodVisitorKey(typeof(Queryable), "First"), (visitor, node) => visitor.VisitFirst(node) },
                { new MethodVisitorKey(typeof(Queryable), "FirstOrDefault"), (visitor, node) => visitor.VisitFirst(node) },
                { new MethodVisitorKey(typeof(Queryable), "Select"), (visitor, node) => visitor.VisitSelect(node) },
                { new MethodVisitorKey(typeof(Queryable), "Where"), (visitor, node) => visitor.VisitWhere(node) },
                { new MethodVisitorKey(typeof(Queryable), "Except"), (visitor, node) => visitor.VisitExcept(node) },
                { new MethodVisitorKey(typeof(Queryable), "OrderBy"), (visitor, node) => visitor.VisitOrderBy(node) },
                { new MethodVisitorKey(typeof(Queryable), "OrderByDescending"), (visitor, node) => visitor.VisitOrderByDescending(node) },
                { new MethodVisitorKey(typeof(Queryable), "Contains"), (visitor, node) => visitor.VisitContains(node) },
                { new MethodVisitorKey(typeof(Queryable), "Skip"), (visitor, node) => visitor.VisitSkip(node) },
                { new MethodVisitorKey(typeof(Queryable), "Take"), (visitor, node) => visitor.VisitTake(node) },
                //Collection
                { new MethodVisitorKey(typeof(ICollection<>), "Contains"), (visitor, node) => visitor.VisitContains(node) },
                //String
                { new MethodVisitorKey(typeof(string), "StartsWith"), (visitor, node) => ExpressionHelper.String.VisitStartsWith(visitor, node) },
                { new MethodVisitorKey(typeof(string), "EndsWith"), (visitor, node) => ExpressionHelper.String.VisitEndsWith(visitor, node) },
                { new MethodVisitorKey(typeof(string), "Contains"), (visitor, node) => ExpressionHelper.String.VisitContains(visitor, node) },
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
                { ExpressionType.Add, QueryOperator.Plus },
                { ExpressionType.Subtract, QueryOperator.Minus }
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

        public IQueryGraphBuilder Query { get; private set; }

        public ITableConfig Table
        {
            get
            {
                return this.Database.Config.GetTable(TableConfig.By(this.ElementType));
            }
        }

        public DatabaseParameterHandler Parameters
        {
            get
            {
                return (parameters, phase) =>
                {
                    foreach (var key in this.Constants.Keys)
                    {
                        if (parameters.Contains(key))
                        {
                            parameters[key] = this.Constants[key];
                        }
                    }
                };
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

        protected virtual bool TryGetTable(Expression expression, out ITableConfig result)
        {
            var type = expression.Type;
            if (type.IsGenericType)
            {
                type = type.GetGenericArguments()[0];
            }
            result = this.Database.Config.GetTable(TableConfig.By(type));
            return result != null;
        }

        protected virtual bool TryGetTable(MemberInfo member, out ITableConfig result)
        {
            var type = this.GetMemberType(member);
            if (type.IsGenericType)
            {
                result = default(ITableConfig);
                return false;
            }
            result = this.Database.Config.GetTable(TableConfig.By(type));
            return result != null;
        }

        protected virtual bool TryGetRelation(MemberInfo member, Expression expression, out IRelationConfig result)
        {
            var type = this.GetMemberType(member);
            if (type.IsGenericType)
            {
                type = type.GetGenericArguments()[0];
            }
            var table = this.Database.Config.GetTable(TableConfig.By(expression.Type));
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

        protected virtual bool TryGetColumn(MemberInfo member, Expression expression, out IColumnConfig result)
        {
            var property = member as PropertyInfo;
            if (property == null)
            {
                result = default(IColumnConfig);
                return false;
            }
            var table = this.Database.Config.GetTable(TableConfig.By(expression.Type));
            if (table == null)
            {
                result = default(IColumnConfig);
                return false;
            }
            result = table.GetColumn(ColumnConfig.By(property));
            return result != null;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var type = default(Type);
            if (node.Method.DeclaringType.IsGenericType)
            {
                type = node.Method.DeclaringType.GetGenericTypeDefinition();
            }
            else
            {
                type = node.Method.DeclaringType;
            }
            var key = new MethodVisitorKey(type, node.Method.Name);
            var handler = default(MethodVisitorHandler);
            if (!MethodHandlers.TryGetValue(key, out handler))
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
                var value = default(object);
                try
                {
                    value = lambda.DynamicInvoke();
                }
                catch (TargetInvocationException e)
                {
                    throw e.InnerException;
                }
                this.VisitParameter(node.Method.ReturnType, value);
            }
            catch (Exception e)
            {
                throw new NotImplementedException(string.Format("The method \"{0}\" of type \"{1}\" is unsupported and could not be evaluated.", node.Method.Name, node.Method.DeclaringType), e);
            }
        }

        protected virtual void VisitQueryable(IQueryable queryable)
        {
            if (object.ReferenceEquals(this.Provider, queryable))
            {
                return;
            }
            if (!(queryable is IDatabaseSetQuery))
            {
                this.VisitEnumerable(queryable);
                return;
            }
            //Nothing to do.
        }

        protected virtual void VisitEnumerable(IEnumerable enumerable)
        {
            if (object.ReferenceEquals(this.Provider, enumerable))
            {
                return;
            }
            this.Peek.Write(this.Push(this.Peek.CreateSequence()));
            try
            {
                var success = false;
                foreach (var element in enumerable)
                {
                    this.VisitParameter(this.Table.PrimaryKey.ColumnType.Type, this.Table.PrimaryKey.Getter(element));
                    success = true;
                }
                if (!success)
                {
                    this.VisitParameter(this.Table.PrimaryKey.ColumnType.Type, this.Table.PrimaryKey.DefaultValue);
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
            else if (this.TryGetRelation(member, expression, out relation))
            {
                this.VisitRelation(relation);
                return true;
            }
            else if (this.TryGetColumn(member, expression, out column))
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
            return this.Peek.Write(this.Peek.CreateColumn(column).With(builder =>
            {
                builder.Alias = column.Identifier;
                builder.Direction = this.Direction;
            }));
        }

        protected virtual IParameterBuilder VisitParameter(Type type, object value)
        {
            return this.VisitParameter(TypeHelper.GetDbType(type), value);
        }

        protected virtual IParameterBuilder VisitParameter(DbType type, object value)
        {
            var name = this.GetParameterName();
            var parameter = this.Peek.Write(this.Peek.CreateParameter(
                name,
                type,
                0,
                0,
                0,
                ParameterDirection.Input,
                false,
                null,
                DatabaseQueryParameterFlags.None
            ));
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
                    this.VisitParameter(node.Type, node.Value);
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
                this.VisitParameter(this.GetMemberType(member), this.GetMemberValue(member, value));
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
                return (member as PropertyInfo).GetValue(value, null);
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

        public class MethodVisitorKey : IEquatable<MethodVisitorKey>
        {
            public MethodVisitorKey(Type type, string name)
            {
                this.Type = type;
                this.Name = name;
            }

            public Type Type { get; private set; }

            public string Name { get; private set; }

            public override int GetHashCode()
            {
                var hashCode = 0;
                unchecked
                {
                    if (this.Type != null)
                    {
                        hashCode += this.Type.GetHashCode();
                    }
                    if (!string.IsNullOrEmpty(this.Name))
                    {
                        hashCode += this.Name.GetHashCode();
                    }
                }
                return hashCode;
            }

            public override bool Equals(object obj)
            {
                if (obj is MethodVisitorKey)
                {
                    return this.Equals(obj as MethodVisitorKey);
                }
                return base.Equals(obj);
            }

            public bool Equals(MethodVisitorKey other)
            {
                if (other == null)
                {
                    return false;
                }
                if (this.Type != other.Type)
                {
                    return false;
                }
                if (!string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                return true;
            }

            public static bool operator ==(MethodVisitorKey a, MethodVisitorKey b)
            {
                if ((object)a == null && (object)b == null)
                {
                    return true;
                }
                if ((object)a == null || (object)b == null)
                {
                    return false;
                }
                if (object.ReferenceEquals((object)a, (object)b))
                {
                    return true;
                }
                return a.Equals(b);
            }

            public static bool operator !=(MethodVisitorKey a, MethodVisitorKey b)
            {
                return !(a == b);
            }
        }

        public delegate void MethodVisitorHandler(EnumerableVisitor visitor, MethodCallExpression node);

        public delegate void UnaryVisitorHandler(EnumerableVisitor visitor, UnaryExpression node);
    }
}
