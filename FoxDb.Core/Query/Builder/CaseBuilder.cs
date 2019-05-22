using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class CaseBuilder : ExpressionBuilder, ICaseBuilder
    {
        public CaseBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {
            this.Expressions = new List<IFragmentBuilder>();
            this.Constants = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Case;
            }
        }

        public ICollection<IFragmentBuilder> Expressions { get; private set; }

        public IDictionary<string, object> Constants { get; private set; }

        public ICaseConditionBuilder Add()
        {
            return this.Add(null, null);
        }

        public ICaseConditionBuilder Add(IFragmentBuilder result)
        {
            return this.Add(null, result);
        }

        public ICaseConditionBuilder Add(IFragmentBuilder condition, IFragmentBuilder result)
        {
            var expression = this.Fragment<ICaseConditionBuilder>();
            expression.Condition = condition;
            expression.Result = result;
            this.Expressions.Add(expression);
            return expression;
        }

        public T Write<T>(T fragment) where T : IFragmentBuilder
        {
            this.Expressions.Add(fragment);
            return fragment;
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<ICaseBuilder>().With(builder =>
            {
                builder.Alias = this.Alias;
                foreach (var expression in this.Expressions)
                {
                    builder.Expressions.Add(expression.Clone());
                }
                foreach (var constant in this.Constants)
                {
                    builder.Constants.Add(constant.Key, constant.Value);
                }
            });
        }
    }
}
