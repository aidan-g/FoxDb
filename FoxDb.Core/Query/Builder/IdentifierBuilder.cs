using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class IdentifierBuilder : ExpressionBuilder, IIdentifierBuilder
    {
        public IdentifierBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Identifier;
            }
        }

        public string Identifier { get; set; }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IIdentifierBuilder>().With(builder =>
            {
                builder.Identifier = this.Identifier;
                builder.Alias = this.Alias;
            });
        }

        public override bool Equals(IFragmentBuilder obj)
        {
            var other = obj as IIdentifierBuilder;
            if (other == null || !base.Equals(obj))
            {
                return false;
            }
            if (!string.Equals(this.Identifier, other.Identifier, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }
    }
}
