using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FoxDb
{
    public class QueryGraphBuilder : FragmentBuilder, IQueryGraphBuilder
    {
        public QueryGraphBuilder()
        {
            this.Fragments = new List<IFragmentBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICollection<IFragmentBuilder> Fragments { get; private set; }

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
                fragment = this.CreateFragment<T>();
                this.Fragments.Add(fragment);
            }
            return fragment;
        }


        public IQueryGraph Build()
        {
            return new QueryGraph(this.Fragments);
        }

        public IQueryGraphBuilder Clone()
        {
            throw new NotImplementedException();
        }
    }
}
