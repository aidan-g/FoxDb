namespace FoxDb
{
    public class SQLiteQuery : DatabaseQuery
    {
        public SQLiteQuery(string commandText, params string[] parameterNames) : base(commandText, parameterNames)
        {
        }
    }
}
