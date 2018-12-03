using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FoxDb
{
    public class BinaryExpressionBuilder : ExpressionBuilder, IBinaryExpressionBuilder
    {
        public BinaryExpressionBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {
            this.Constants = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Binary;
            }
        }

        public ICollection<IFragmentBuilder> Expressions
        {
            get
            {
                return new ReadOnlyCollection<IFragmentBuilder>(new IFragmentBuilder[]
                {
                    this.Left,
                    this.Operator,
                    this.Right
                }.Where(expression => expression != null).ToList());
            }
        }

        public IDictionary<string, object> Constants { get; private set; }

        public bool IsLeaf
        {
            get
            {
                return !(this.Left is IFragmentContainer) && !(this.Right is IFragmentContainer);
            }
        }

        public IFragmentBuilder Left { get; set; }

        public IOperatorBuilder Operator { get; set; }

        public IFragmentBuilder Right { get; set; }

        public T SetLeft<T>(T expression) where T : IFragmentBuilder
        {
            this.Left = expression;
            return expression;
        }

        public T SetRight<T>(T expression) where T : IFragmentBuilder
        {
            this.Right = expression;
            return expression;
        }

        public T Write<T>(T fragment) where T : IFragmentBuilder
        {
            if (this.Left == null)
            {
                this.Left = fragment;
                return fragment;
            }
            else if (this.Operator == null)
            {
                if (fragment is IOperatorBuilder)
                {
                    this.Operator = fragment as IOperatorBuilder;
                    return fragment;
                }
            }
            else if (this.Right == null)
            {
                this.Right = fragment;
                return fragment;
            }
            throw new NotImplementedException();
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IBinaryExpressionBuilder>().With(builder =>
            {
                if (this.Left != null)
                {
                    builder.Left = this.Left.Clone();
                }
                if (this.Operator != null)
                {
                    builder.Operator = (IOperatorBuilder)this.Operator.Clone();
                }
                if (this.Right != null)
                {
                    builder.Right = this.Right.Clone();
                }
                foreach (var constant in this.Constants)
                {
                    builder.Constants.Add(constant.Key, constant.Value);
                }
            });
        }

        public override bool Equals(IFragmentBuilder obj)
        {
            var other = obj as IBinaryExpressionBuilder;
            if (other == null || !base.Equals(obj))
            {
                return false;
            }
            if (this.Left != other.Left || this.Operator != other.Operator || this.Right != other.Right)
            {
                return false;
            }
            return true;
        }
    }
}
