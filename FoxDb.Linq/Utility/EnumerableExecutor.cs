using System;
using System.Linq.Expressions;

namespace FoxDb
{
    public abstract class EnumerableExecutor
    {
        protected EnumerableExecutor(Expression expression)
        {
            this.Expression = expression;
        }

        public abstract Type ElementType { get; }

        public Expression Expression { get; private set; }
    }

    public class EnumerableExecutor<T> : EnumerableExecutor
    {
        public EnumerableExecutor(Expression expression) : base(expression)
        {

        }

        public override Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        public Func<T> Func { get; private set; }

        public T Execute()
        {
            if (this.Func == null)
            {
                var lambda = Expression.Lambda<Func<T>>(this.Expression);
                this.Func = lambda.Compile();
            }
            return this.Func();
        }

        public static T Execute(Expression expression)
        {
            return new EnumerableExecutor<T>(expression).Execute();
        }
    }
}
