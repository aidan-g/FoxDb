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

    public partial class EnumerableQuery<T> : EnumerableQuery, IEnumerableQuery<T>
    {
        private EnumerableQuery()
        {
            this.Members = new DynamicMethod<EnumerableQuery<T>>();
        }

        public EnumerableQuery(IDatabaseSetQuery query, IDatabaseSet set) : this()
        {
            this.Query = query;
            this.Set = set;
        }

        public EnumerableQuery(IDatabaseSetQuery query, IDatabaseSet set, Expression expression) : this()
        {
            this.Query = query;
            this.Set = set;
            this.Expression = expression;
        }

        protected DynamicMethod<EnumerableQuery<T>> Members { get; private set; }

        public override Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        public IDatabaseSetQuery Query { get; private set; }

        public IDatabaseSet Set { get; private set; }

        public IQueryProvider Provider
        {
            get
            {
                return this.Query;
            }
        }

        public Expression Expression { get; private set; }

        public IEnumerable<T> Sequence { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            if (this.Sequence == null)
            {
                this.Sequence = EnumerableExecutor<IEnumerable<T>>.Execute(EnumerableRewriter.Rewrite(this.Query, this.Set, this.Expression));
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
            return EnumerableExecutor<TResult>.Execute(EnumerableRewriter.Rewrite(this.Query, this.Set, expression));
        }
    }

    public partial class EnumerableQuery<T>
    {
        int ICollection.Count
        {
            get
            {
                var collection = (ICollection)this.Set;
                return collection.Count;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                var collection = (ICollection)this.Set;
                return collection.SyncRoot;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                var collection = (ICollection)this.Set;
                return collection.IsSynchronized;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            var collection = (ICollection)this.Set;
            collection.CopyTo(array, index);
        }
    }

    public partial class EnumerableQuery<T>
    {
        void ICollection<T>.Add(T item)
        {
            var collection = (ICollection<T>)this.Set;
            collection.Add(item);
        }

        void ICollection<T>.Clear()
        {
            var collection = (ICollection<T>)this.Set;
            collection.Clear();
        }

        bool ICollection<T>.Contains(T item)
        {
            var collection = (ICollection<T>)this.Set;
            return collection.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int index)
        {
            var collection = (ICollection<T>)this.Set;
            collection.CopyTo(array, index);
        }

        int ICollection<T>.Count
        {
            get
            {
                var collection = (ICollection<T>)this.Set;
                return collection.Count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                var collection = (ICollection<T>)this.Set;
                return collection.IsReadOnly;
            }
        }

        bool ICollection<T>.Remove(T item)
        {
            var collection = (ICollection<T>)this.Set;
            return collection.Remove(item);
        }
    }

    public partial class EnumerableQuery<T>
    {
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator()
        {
            if (!this.Set.ElementType.IsAssignableFrom(typeof(T)))
            {
                throw new NotImplementedException();
            }
            return (IAsyncEnumerator<T>)this.Set.GetAsyncEnumerator();
        }
    }
}
