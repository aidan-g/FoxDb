using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlOrderByRewriter : SqlQueryRewriter
    {
        public SqlOrderByRewriter(IDatabase database)
            : base(database)
        {
        }
    }
}
