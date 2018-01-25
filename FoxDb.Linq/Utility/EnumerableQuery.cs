using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FoxDb
{
    public abstract class EnumerableQuery
    {
        public abstract Type ElementType { get; }
    }

    public class EnumerableQuery<T> : EnumerableQuery, IEnumerableQuery<T>
    {
        public EnumerableQuery(IDatabaseSetQuery provider) : this(provider, null)
        {
        }

        public EnumerableQuery(IDatabaseSetQuery provider, Expression expression)
        {
            this.Provider = provider;
            this.Expression = expression;
        }

        public override Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        public IDatabaseSetQuery Provider { get; private set; }

        IQueryProvider IQueryable.Provider
        {
            get
            {
                return this.Provider;
            }
        }

        public Expression Expression { get; private set; }

        public IEnumerable<T> Sequence { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            if (this.Sequence == null)
            {
                this.Sequence = EnumerableExecutor<IEnumerable<T>>.Execute(EnumerableRewriter.Rewrite(this.Provider, this.Expression));
            }
            return this.Sequence.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return EnumerableExecutor<TResult>.Execute(EnumerableRewriter.Rewrite(this.Provider, expression));
        }
    }
}
