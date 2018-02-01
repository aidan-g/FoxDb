using System;

namespace FoxDb
{
    public class SQLiteQueryDialect : SqlQueryDialect
    {
        public override string IDENTITY
        {
            get
            {
                return "LAST_INSERT_ROWID";
            }
        }

        public override string BATCH
        {
            get
            {
                return string.Format("{0};", Environment.NewLine);
            }
        }
    }
}
