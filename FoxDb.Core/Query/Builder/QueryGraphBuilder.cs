using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FoxDb
{
    public class QueryGraphBuilder : IQueryGraphBuilder
    {
        private QueryGraphBuilder()
        {
            this.Fragments = new List<IFragmentBuilder>();
        }

        public QueryGraphBuilder(IDatabase database) : this()
        {
            this.Database = database;
        }

        public ICollection<IFragmentBuilder> Fragments { get; private set; }

        public IDatabase Database { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IOutputBuilder Output
        {
            get
            {
                return this.Fragment<IOutputBuilder>();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IAddBuilder Add
        {
            get
            {
                return this.Fragment<IAddBuilder>();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IUpdateBuilder Update
        {
            get
            {
                return this.Fragment<IUpdateBuilder>();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IDeleteBuilder Delete
        {
            get
            {
                return this.Fragment<IDeleteBuilder>();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ISourceBuilder Source
        {
            get
            {
                return this.Fragment<ISourceBuilder>();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IFilterBuilder Filter
        {
            get
            {
                return this.Fragment<IFilterBuilder>();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IAggregateBuilder Aggregate
        {
            get
            {
                return this.Fragment<IAggregateBuilder>();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ISortBuilder Sort
        {
            get
            {
                return this.Fragment<ISortBuilder>();
            }
        }

        public virtual T Fragment<T>() where T : IFragmentBuilder
        {
            var fragment = this.Fragments.OfType<T>().FirstOrDefault();
            if (fragment == null)
            {
                fragment = FragmentBuilder.GetProxy(this).Fragment<T>();
                this.Fragments.Add(fragment);
            }
            return fragment;
        }


        public IDatabaseQuery Build()
        {
            return this.Database.QueryFactory.Create(new[]
            {
                new QueryGraph(this.Fragments)
            });
        }

        public IQueryGraphBuilder Clone()
        {
            throw new NotImplementedException();
        }

        public static IQueryGraphBuilder Null
        {
            get
            {
                return new NullQueryGraphBuilder();
            }
        }

        private class NullQueryGraphBuilder : IQueryGraphBuilder
        {
            public IOutputBuilder Output
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IAddBuilder Add
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IUpdateBuilder Update
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IDeleteBuilder Delete
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public ISourceBuilder Source
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IFilterBuilder Filter
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IAggregateBuilder Aggregate
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public ISortBuilder Sort
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IDatabaseQuery Build()
            {
                throw new NotImplementedException();
            }

            public IQueryGraphBuilder Clone()
            {
                throw new NotImplementedException();
            }

            public T Fragment<T>() where T : IFragmentBuilder
            {
                throw new NotImplementedException();
            }
        }
    }

    public class AggregateQueryGraphBuilder : IAggregateQueryGraphBuilder
    {
        private AggregateQueryGraphBuilder(IDatabase database)
        {
            this.Database = database;
        }

        public AggregateQueryGraphBuilder(IDatabase database, params IQueryGraphBuilder[] queries) : this(database)
        {
            this.Queries = queries;
        }

        public IDatabase Database { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IEnumerable<IQueryGraphBuilder> Queries { get; private set; }

        public IDatabaseQuery Build()
        {
            return this.Database.QueryFactory.Combine(this.Select(query => query.Build()).ToArray());
        }

        public IEnumerator<IQueryGraphBuilder> GetEnumerator()
        {
            return this.Queries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #region IQueryGraphBuilder

        IOutputBuilder IQueryGraphBuilder.Output
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IAddBuilder IQueryGraphBuilder.Add
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IUpdateBuilder IQueryGraphBuilder.Update
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IDeleteBuilder IQueryGraphBuilder.Delete
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        ISourceBuilder IQueryGraphBuilder.Source
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IFilterBuilder IQueryGraphBuilder.Filter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IAggregateBuilder IQueryGraphBuilder.Aggregate
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        ISortBuilder IQueryGraphBuilder.Sort
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        T IQueryGraphBuilder.Fragment<T>()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
