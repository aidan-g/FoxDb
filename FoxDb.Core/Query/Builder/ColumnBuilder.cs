using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FoxDb
{
    public class ColumnBuilder : ExpressionBuilder, IColumnBuilder
    {
        public ColumnBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {
            this.Direction = OrderByDirection.None;
            this.Flags = ColumnBuilderFlags.None;
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Column;
            }
        }

        public ICollection<IFragmentBuilder> Expressions
        {
            get
            {
                var expressions = new List<IFragmentBuilder>();
                if (this.Column != null)
                {
                    expressions.Add(this.CreateTable(this.Column.Table));
                }
                return new ReadOnlyCollection<IFragmentBuilder>(expressions);
            }
        }

        public OrderByDirection Direction { get; set; }

        public IColumnConfig Column { get; set; }

        public bool UseIdentifier { get; set; }

        public ColumnBuilderFlags Flags { get; set; }

        public override string DebugView
        {
            get
            {
                return string.Format("{{{0}}}", this.Column);
            }
        }

        public override bool Equals(IFragmentBuilder obj)
        {
            var other = obj as IColumnBuilder;
            if (other == null || !base.Equals(obj))
            {
                return false;
            }
            if (this.Column != other.Column)
            {
                return false;
            }
            return true;
        }
    }
}
