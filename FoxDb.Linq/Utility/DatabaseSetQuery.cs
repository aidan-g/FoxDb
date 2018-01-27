﻿using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FoxDb
{
    public abstract class DatabaseSetQuery
    {
        public abstract Type ElementType { get; }

        public abstract IQueryProvider Provider { get; }
    }

    public class DatabaseSetQuery<T> : DatabaseSetQuery, IDatabaseSetQuery<T>
    {
        public DatabaseSetQuery(IDatabaseSet<T> set)
        {
            this.Set = set;
            this.Expression = Expression.Constant(this);
        }

        public DatabaseSetQuery(IDatabaseSet set, Expression expression)
        {
            this.Set = set;
            this.Expression = expression;
        }

        public IDatabaseSet Set { get; private set; }

        public override Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        public override IQueryProvider Provider
        {
            get
            {
                return this;
            }
        }

        public Expression Expression { get; private set; }

        public IEnumerable<T> Sequence { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            if (this.Sequence == null)
            {
                this.Sequence = this.CreateQuery<T>(this.Expression);
            }
            return this.Sequence.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            this.ConfigureSet(expression);
            return new EnumerableQuery<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            this.ConfigureSet(expression);
            return new EnumerableQuery<T>(this).Execute<TResult>(expression);
        }

        public void Reset()
        {
            this.Set.Reset();
        }

        protected virtual void ConfigureSet(Expression expression)
        {
            var visitor = new EnumerableVisitor(this.Set.Database, this.Set.Fetch, this.Set.ElementType);
            visitor.Visit(expression);
            this.Set.Parameters = visitor.Parameters;
        }
    }
}