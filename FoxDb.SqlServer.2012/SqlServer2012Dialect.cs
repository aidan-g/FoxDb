using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlServer2012Dialect : SqlServerQueryDialect
    {
        public SqlServer2012Dialect(IDatabase database)
            : base(database)
        {

        }

        public string FETCH
        {
            get
            {
                return "FETCH";
            }
        }

        public string NEXT
        {
            get
            {
                return "NEXT";
            }
        }

        public string ROWS
        {
            get
            {
                return "ROWS";
            }
        }

        public string ONLY
        {
            get
            {
                return "ONLY";
            }
        }
    }
}
