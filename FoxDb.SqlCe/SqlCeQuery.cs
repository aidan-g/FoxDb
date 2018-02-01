using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlCeQuery : DatabaseQuery
    {
        public SqlCeQuery(string commandText, params IDatabaseQueryParameter[] parameters) : base(commandText, parameters)
        {
        }
    }
}
