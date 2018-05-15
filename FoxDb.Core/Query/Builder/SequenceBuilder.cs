﻿using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SequenceBuilder : FragmentBuilder, ISequenceBuilder
    {
        public SequenceBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
            : base(parent, graph)
        {
            this.Expressions = new List<IFragmentBuilder>();
            this.Constants = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Sequence;
            }
        }

        public ICollection<IFragmentBuilder> Expressions { get; private set; }

        public IDictionary<string, object> Constants { get; private set; }

        public IParameterBuilder AddParameter(string name)
        {
            var expression = this.CreateParameter(name);
            this.Expressions.Add(expression);
            return expression;
        }

        public IParameterBuilder AddParameter(IColumnConfig column)
        {
            var expression = this.CreateParameter(Conventions.ParameterName(column));
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
            return this.Parent.Fragment<ISequenceBuilder>().With(builder =>
            {
                foreach (var expression in this.Expressions)
                {
                    builder.Expressions.Add(expression.Clone());
                }
            });
        }
    }
}
