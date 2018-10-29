using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlCeQueryTypes : SqlQueryTypes
    {
        static SqlCeQueryTypes()
        {
            Arguments["nvarchar"] = DatabaseQueryTypeArguments.Size;
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
