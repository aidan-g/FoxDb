namespace FoxDb
{
    public class SqlCeQuery : DatabaseQuery
    {
        public SqlCeQuery(string commandText, params string[] parameterNames) : base(commandText, parameterNames)
        {
        }
    }
}
