using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlSelectRewriter : SqlQueryRewriter
    {
        public SqlSelectRewriter(IDatabase database)
            : base(database)
        {
        }
    }
}
