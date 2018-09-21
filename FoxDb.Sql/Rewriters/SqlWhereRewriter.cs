using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlWhereRewriter : SqlQueryRewriter
    {
        public SqlWhereRewriter(IDatabase database)
            : base(database)
        {
        }
    }
}
