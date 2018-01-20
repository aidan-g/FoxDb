﻿using FoxDb.Interfaces;

namespace FoxDb
{
    public class RelationBuilder : ExpressionBuilder, IRelationBuilder
    {
        public RelationBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Relation;
            }
        }

        public IRelationConfig Relation { get; set; }

        public override string DebugView
        {
            get
            {
                return string.Format("{{{0}}}", this.Relation);
            }
        }
    }
}
