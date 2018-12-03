using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FoxDb
{
    public class TableBuilder : ExpressionBuilder, ITableBuilder
    {
        public TableBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
            : base(parent, graph)
        {
            this.Expressions = new List<IFragmentBuilder>();
            this.Constants = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this.Filter = this.Fragment<IFilterBuilder>();
            this.Sort = this.Fragment<ISortBuilder>();
            this.LockHints = LockHints.None;
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Table;
            }
        }

        public ICollection<IFragmentBuilder> Expressions { get; private set; }

        public IDictionary<string, object> Constants { get; private set; }

        public ITableConfig Table { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IFilterBuilder Filter { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ISortBuilder Sort { get; set; }

        public LockHints LockHints { get; set; }

        public T Write<T>(T fragment) where T : IFragmentBuilder
        {
            this.Expressions.Add(fragment);
            return fragment;
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<ITableBuilder>().With(builder =>
            {
                builder.Table = this.Table;
                if (this.Filter != null)
                {
                    builder.Filter = (IFilterBuilder)this.Filter.Clone();
                }
                if (this.Sort != null)
                {
                    builder.Sort = (ISortBuilder)this.Sort.Clone();
                }
                builder.LockHints = this.LockHints;
                foreach (var constant in this.Constants)
                {
                    builder.Constants.Add(constant.Key, constant.Value);
                }
                foreach (var constant in this.Constants)
                {
                    builder.Constants.Add(constant.Key, constant.Value);
                }
            });
        }

        public override bool Equals(IFragmentBuilder obj)
        {
            var other = obj as ITableBuilder;
            if (other == null || !base.Equals(obj))
            {
                return false;
            }
            if ((TableConfig)this.Table != (TableConfig)other.Table)
            {
                return false;
            }
            return true;
        }
    }
}
