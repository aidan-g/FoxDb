﻿using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class QueryGraph : IQueryGraph
    {
        public QueryGraph(IEnumerable<IFragmentBuilder> fragments)
        {
            this.Fragments = fragments;
        }

        public IEnumerable<IFragmentBuilder> Fragments { get; private set; }
    }
}