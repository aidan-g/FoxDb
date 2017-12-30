using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class BinaryExpressionBuilder : ExpressionBuilder, IBinaryExpressionBuilder
    {
        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Binary;
            }
        }

        public IExpressionBuilder Left { get; set; }

        public IOperatorBuilder Operator { get; set; }

        public IExpressionBuilder Right { get; set; }

        public void Write(IFragmentBuilder fragment)
        {
            if (this.Left == null)
            {
                if (fragment is IExpressionBuilder)
                {
                    this.Left = fragment as IExpressionBuilder;
                    return;
                }
            }
            if (this.Operator == null)
            {
                if(fragment is IOperatorBuilder)
                {
                    this.Operator = fragment as IOperatorBuilder;
                    return;
                }
            }
            if (this.Right == null)
            {
                if (fragment is IExpressionBuilder)
                {
                    this.Right = fragment as IExpressionBuilder;
                    return;
                }
            }
            throw new NotImplementedException();
        }
    }
}
