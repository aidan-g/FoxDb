﻿using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class LimitBuilder : FragmentBuilder, ILimitBuilder
    {
        public LimitBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph, int limit) : base(parent, graph)
        {
            this.Limit = limit;
        }

        public override FragmentType FragmentType
        {
            get
            {
                return SQLiteQueryFragment.Limit;
            }
        }

        public int Limit { get; private set; }

        public override IFragmentBuilder Clone()
        {
            throw new NotImplementedException();
        }
    }
}
