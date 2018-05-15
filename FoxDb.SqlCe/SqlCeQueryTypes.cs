using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlCeQueryTypes : SqlQueryTypes
    {
        static SqlCeQueryTypes()
        {
            Arguments = new Dictionary<string, DatabaseQueryTypeArguments>(Arguments)
            {
                { "nvarchar", DatabaseQueryTypeArguments.Size }
            };
        }

        public SqlCeQueryTypes(IDatabase database)
            : base(database)
        {

        }

        protected override string DefaultStringType
        {
            get
            {
                return "nvarchar";
            }
        }

        protected override int DefaultSize
        {
            get
            {
                return 50;
            }
        }
    }
}
