using FoxDb.Interfaces;

namespace FoxDb
{
    public class SQLiteQueryTypes : SqlQueryTypes
    {
        public SQLiteQueryTypes(IDatabase database) : base(database)
        {

        }

        protected override string DefaultBooleanType
        {
            get
            {
                return this.DefaultIntegralType;
            }
        }
    }
}
