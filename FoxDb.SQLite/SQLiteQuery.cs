using FoxDb.Interfaces;

namespace FoxDb
{
    public class SQLiteQuery : DatabaseQuery
    {
        public SQLiteQuery(string commandText, params IDatabaseQueryParameter[] parameters) : base(commandText, parameters)
        {
        }
    }
}
