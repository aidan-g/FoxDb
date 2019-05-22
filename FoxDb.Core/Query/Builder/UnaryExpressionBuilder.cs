using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FoxDb
{
    public class UnaryExpressionBuilder : ExpressionBuilder, IUnaryExpressionBuilder
    {
        public UnaryExpressionBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {
            this.Constants = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Unary;
            }
        }

        public ICollection<IFragmentBuilder> Expressions
        {
            get
            {
                return new ReadOnlyCollection<IFragmentBuilder>(new IFragmentBuilder[]
                {
                    this.Operator,
                    this.Expression
                }.ToList());
            }
        }

        public IDictionary<string, object> Constants { get; private set; }

        public bool IsLeaf
        {
            get
            {
                return !(this.Expression is IFragmentContainer);
            }
        }

        public IOperatorBuilder Operator { get; set; }

        public IFragmentBuilder Expression { get; set; }

        public T Write<T>(T fragment) where T : IFragmentBuilder
        {
            if (this.Operator == null)
            {
                if (fragment is IOperatorBuilder)
                {
                    this.Operator = fragment as IOperatorBuilder;
                    return fragment;
                }
            }
            else if (this.Expression == null)
            {
                this.Expression = fragment;
                return fragment;
            }
            throw new NotImplementedException();
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IUnaryExpressionBuilder>().With(builder =>
            {
                builder.Operator = (IOperatorBuilder)this.Operator.Clone();
                builder.Expression = this.Expression.Clone();
                builder.Alias = this.Alias;
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
            var other = obj as IUnaryExpressionBuilder;
            if (other == null || !base.Equals(obj))
            {
                return false;
            }
            if (this.Operator != other.Operator || this.Expression != other.Expression)
            {
                return false;
            }
            return true;
        }
    }
}
