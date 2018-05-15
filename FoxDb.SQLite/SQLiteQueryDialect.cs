using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class SQLiteQueryDialect : SqlQueryDialect
    {
        public SQLiteQueryDialect(IDatabase database)
            : base(database)
        {

        }

        public string LAST_INSERT_ROWID
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
