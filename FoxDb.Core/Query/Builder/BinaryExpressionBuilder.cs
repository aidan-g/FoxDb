using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class BinaryExpressionBuilder : ExpressionBuilder, IBinaryExpressionBuilder
    {
        public BinaryExpressionBuilder()
        {
            this.Constants = new Dictionary<string, object>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Binary;
            }
        }

        public IDictionary<string, object> Constants { get; private set; }

        public IExpressionBuilder Left { get; set; }

        public IOperatorBuilder Operator { get; set; }

        public IExpressionBuilder Right { get; set; }

        public T Write<T>(T fragment) where T : IFragmentBuilder
        {
            if (this.Left == null)
            {
                if (fragment is IExpressionBuilder)
                {
                    this.Left = fragment as IExpressionBuilder;
                    return fragment;
                }
            }
            if (this.Operator == null)
            {
                if (fragment is IOperatorBuilder)
                {
                    this.Operator = fragment as IOperatorBuilder;
                    return fragment;
                }
            }
            if (this.Right == null)
            {
                if (fragment is IExpressionBuilder)
                {
                    this.Right = fragment as IExpressionBuilder;
                    return fragment;
                }
            }
            throw new NotImplementedException();
        }
    }
}
