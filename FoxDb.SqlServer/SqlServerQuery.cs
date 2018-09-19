using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlServerQuery : DatabaseQuery
    {
        public SqlServerQuery(string commandText, params IDatabaseQueryParameter[] parameters)
            : base(commandText, parameters)
        {
        }
    }
}
