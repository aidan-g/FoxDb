using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class IndexBuilder : ExpressionBuilder, IIndexBuilder
    {
        public IndexBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
            : base(parent, graph)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Index;
            }
        }

        public IIndexConfig Index { get; set; }

        public ITableBuilder Table
        {
            get
            {
                return this.CreateTable(this.Index.Table);
            }
        }

        public IEnumerable<IIdentifierBuilder> Columns
        {
            get
            {
                if (this.Index != null)
                {
                    foreach (var column in this.Index.Columns)
                    {
                        yield return this.CreateIdentifier(column.ColumnName);
                    }
                }
            }
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IIndexBuilder>().With(builder =>
            {
                builder.Index = this.Index;
            });
        }

        public override bool Equals(IFragmentBuilder obj)
        {
            var other = obj as IIndexBuilder;
            if (other == null || !base.Equals(obj))
            {
                return false;
            }
            if (this.Index != other.Index)
            {
                return false;
            }
            return true;
        }
    }
}
