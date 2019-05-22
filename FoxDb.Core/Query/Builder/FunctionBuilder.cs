﻿using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class FunctionBuilder : ExpressionBuilder, IFunctionBuilder
    {
        public FunctionBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {
            this.Expressions = new List<IFragmentBuilder>();
            this.Constants = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Function;
            }
        }

        public QueryFunction Function { get; set; }

        public ICollection<IFragmentBuilder> Expressions { get; private set; }

        public IDictionary<string, object> Constants { get; private set; }

        public IFunctionBuilder AddArgument(IExpressionBuilder argument)
        {
            this.Expressions.Add(argument);
            return this;
        }

        public IFunctionBuilder AddArguments(IEnumerable<IExpressionBuilder> arguments)
        {
            foreach (var argument in arguments)
            {
                this.AddArgument(argument);
            }
            return this;
        }

        public T Write<T>(T fragment) where T : IFragmentBuilder
        {
            this.Expressions.Add(fragment);
            return fragment;
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IFunctionBuilder>().With(builder =>
            {
                builder.Function = this.Function;
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
