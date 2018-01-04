using FoxDb.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public static class PropertyAccessorFactory
    {
        public static IPropertyAccessor<T, TValue> Create<T, TValue>(Expression expression)
        {
            if (expression.NodeType != ExpressionType.Lambda)
            {
                throw new NotImplementedException();
            }
            var lambda = expression as LambdaExpression;
            if (lambda.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new NotImplementedException();
            }
            var member = lambda.Body as MemberExpression;
            if (!(member.Member is PropertyInfo))
            {
                throw new NotImplementedException();
            }
            var property = member.Member as PropertyInfo;
            var get = property.GetGetMethod().CreateDelegate<Func<T, TValue>>();
            var set = property.GetSetMethod().CreateDelegate<Action<T, TValue>>();
            return new PropertyAccessor<T, TValue>(get, set);
        }

        private class PropertyAccessor<T, TValue> : IPropertyAccessor<T, TValue>
        {
            public PropertyAccessor(Func<T, TValue> get, Action<T, TValue> set)
            {
                this.Get = get;
                this.Set = set;
            }

            public Func<T, TValue> Get { get; private set; }

            public Action<T, TValue> Set { get; private set; }
        }
    }
}
