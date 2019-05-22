using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FoxDb
{
    public class CaseConditionBuilder : ExpressionBuilder, ICaseConditionBuilder
    {
        public CaseConditionBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {
            this.Constants = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.CaseCondition;
            }
        }

        public ICollection<IFragmentBuilder> Expressions
        {
            get
            {
                return new ReadOnlyCollection<IFragmentBuilder>(new IFragmentBuilder[]
                {
                    this.Condition,
                    this.Result
                }.ToList());
            }
        }

        public IDictionary<string, object> Constants { get; private set; }

        public IFragmentBuilder Condition { get; set; }

        public IFragmentBuilder Result { get; set; }

        public T Write<T>(T fragment) where T : IFragmentBuilder
        {
            if (this.Condition == null)
            {
                this.Condition = fragment;
                return fragment;
            }
            else if (this.Result == null)
            {
                this.Result = fragment;
                return fragment;
            }
            throw new NotImplementedException();
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<ICaseConditionBuilder>().With(builder =>
            {
                builder.Condition = this.Condition;
                builder.Result = this.Result;
                builder.Alias = this.Alias;
                foreach (var constant in this.Constants)
                {
                    builder.Constants.Add(constant.Key, constant.Value);
                }
            });
        }
    }
}