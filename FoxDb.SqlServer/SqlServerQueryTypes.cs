using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlServerQueryTypes : SqlQueryTypes
    {
        static SqlServerQueryTypes()
        {
            Arguments["nvarchar"] = DatabaseQueryTypeArguments.Size;
        }

        public SqlServerQueryTypes(IDatabase database)
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

        protected override string DefaultGuidType
        {
            get
            {
                return "uniqueidentifier";
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
