using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class QueryGraphBuilder : IQueryGraphBuilder
    {
        public static readonly IDictionary<Type, Func<IFragmentBuilder>> Factories = new Dictionary<Type, Func<IFragmentBuilder>>()
        {
            { typeof(ISelectBuilder), () => new SelectBuilder() },
            { typeof(IInsertBuilder), () => new InsertBuilder() },
            { typeof(IUpdateBuilder), () => new UpdateBuilder() },
            { typeof(IDeleteBuilder), () => new DeleteBuilder() },
            { typeof(IFromBuilder), () => new FromBuilder() },
            { typeof(IWhereBuilder), () => new WhereBuilder() },
            { typeof(IOrderByBuilder), () => new OrderByBuilder() }
        };

        public QueryGraphBuilder()
        {
            this.Fragments = new List<IFragmentBuilder>();
        }

        public ICollection<IFragmentBuilder> Fragments { get; }

        public ISelectBuilder Select
        {
            get
            {
                return this.Fragment<ISelectBuilder>();
            }
        }

        public IInsertBuilder Insert
        {
            get
            {
                return this.Fragment<IInsertBuilder>();
            }
        }

        public IUpdateBuilder Update
        {
            get
            {
                return this.Fragment<IUpdateBuilder>();
            }
        }

        public IDeleteBuilder Delete
        {
            get
            {
                return this.Fragment<IDeleteBuilder>();
            }
        }

        public IFromBuilder From
        {
            get
            {
                return this.Fragment<IFromBuilder>();
            }
        }

        public IWhereBuilder Where
        {
            get
            {
                return this.Fragment<IWhereBuilder>();
            }
        }

        public IOrderByBuilder OrderBy
        {
            get
            {
                return this.Fragment<IOrderByBuilder>();
            }
        }

        public virtual T Fragment<T>() where T : IFragmentBuilder
        {
            var fragment = this.Fragments.OfType<T>().FirstOrDefault();
            if (fragment == null)
            {
                var factory = default(Func<IFragmentBuilder>);
                if (!Factories.TryGetValue(typeof(T), out factory))
                {
                    throw new NotImplementedException();
                }
                fragment = (T)factory();
                this.Fragments.Add(fragment);
            }
            return fragment;
        }


        public IQueryGraph Build()
        {
            return new QueryGraph(this.Fragments);
        }
    }
}
