﻿using FoxDb.Interfaces;

namespace FoxDb
{
    public class ConstantBuilder : ExpressionBuilder, IConstantBuilder
    {
        public ConstantBuilder(IQueryGraphBuilder graph) : base(graph)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Constant;
            }
        }

        public object Value { get; set; }
    }
}
