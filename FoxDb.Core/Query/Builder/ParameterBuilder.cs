﻿using FoxDb.Interfaces;
using System.Data;

namespace FoxDb
{
    public class ParameterBuilder : ExpressionBuilder, IParameterBuilder
    {
        public ParameterBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
            : base(parent, graph)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Parameter;
            }
        }

        public string Name { get; set; }

        public DbType Type { get; set; }

        public ParameterDirection Direction { get; set; }

        public bool IsDeclared { get; set; }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IParameterBuilder>().With(builder =>
            {
                builder.Name = this.Name;
                builder.Type = this.Type;
                builder.Direction = this.Direction;
                builder.IsDeclared = this.IsDeclared;
            });
        }
    }
}
