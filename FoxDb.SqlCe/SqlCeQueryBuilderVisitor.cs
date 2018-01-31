using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlCeQueryBuilderVisitor : SqlQueryBuilderVisitor
    {
        public SqlCeQueryBuilderVisitor(IDatabase database) : base(database)
        {
        }

        protected override SqlQueryFragment CreateQueryFragment(IFragmentTarget target)
        {
            return new SqlCeQueryFragment(target);
        }
    }
}
