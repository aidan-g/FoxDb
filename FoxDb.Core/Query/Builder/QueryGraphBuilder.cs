using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Concurrent;
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
            this.RelationManager = new RelationManager(this);
            this.Filter.Touch(); //TODO: Relation manager requires filtering.
        }

        public QueryGraphBuilder(IDatabase database)
            : this()
        {
            this.Database = database;
        }

        public ICollection<IFragmentBuilder> Fragments { get; private set; }

        public IDatabase Database { get; private set; }

        public IQueryGraphBuilder Parent { get; set; }

        public IRelationManager RelationManager { get; private set; }

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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IWithBuilder With
        {
            get
            {
                return this.Fragment<IWithBuilder>();
            }
        }

        public virtual T Fragment<T>(T fragment) where T : IFragmentBuilder
        {
            this.Fragments.Add(fragment);
            return fragment;
        }

        public virtual T Fragment<T>() where T : IFragmentBuilder
        {
            var fragment = this.Fragments.OfType<T>().FirstOrDefault();
            if (fragment == null)
            {
                return this.Fragment(FragmentBuilder.GetProxy(this).Fragment<T>());
            }
            return fragment;
        }

        public IDatabaseQuery Build()
        {
            return this.Database.QueryFactory.Create(this);
        }

        public IQueryGraphBuilder Clone()
        {
            var builder = this.Database.QueryFactory.Build();
            if (this.Parent != null)
            {
                builder.Parent = this.Parent;
            }
            foreach (var relation in this.RelationManager.Relations)
            {
                builder.RelationManager.AddRelation(relation);
            }
            builder.Fragments.Clear();
            foreach (var fragment in this.Fragments)
            {
                builder.Fragment(fragment.Clone());
            }
            return builder;
        }
    }

    public class AggregateQueryGraphBuilder : IAggregateQueryGraphBuilder
    {
        private AggregateQueryGraphBuilder(IDatabase database)
        {
            this.Database = database;
        }

        public AggregateQueryGraphBuilder(IDatabase database, IEnumerable<IQueryGraphBuilder> queries)
            : this(database)
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

        public IQueryGraphBuilder Clone()
        {
            return new AggregateQueryGraphBuilder(this.Database, this.Queries.Select(query => query.Clone()).ToArray());
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

        IRelationManager IQueryGraphBuilder.RelationManager
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IQueryGraphBuilder IQueryGraphBuilder.Parent
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

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

        IWithBuilder IQueryGraphBuilder.With
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        ICollection<IFragmentBuilder> IQueryGraphBuilder.Fragments
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

        T IQueryGraphBuilder.Fragment<T>(T fragment)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
