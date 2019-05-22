using FoxDb.Interfaces;

namespace FoxDb
{
    public class SubQueryBuilder : ExpressionBuilder, ISubQueryBuilder
    {
        public SubQueryBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
            : base(parent, graph)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.SubQuery;
            }
        }

        private IQueryGraphBuilder _Query { get; set; }

        public IQueryGraphBuilder Query
        {
            get
            {
                return this._Query;
            }
            set
            {
                this._Query = value;
                this.OnQueryChanged();
            }
        }

        protected virtual void OnQueryChanged()
        {
            if (this.Query == null)
            {
                return;
            }
            this.Query.Parent = this.Graph;
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<ISubQueryBuilder>().With(builder =>
            {
                builder.Query = this.Query.Clone();
                builder.Alias = this.Alias;
            });
        }
    }
}
