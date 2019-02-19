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

        public override IDatabaseQueryTypes Types
        {
            get
            {
                return new SQLiteQueryTypes(this.Database);
            }
        }

        public override string LAST_INSERT_ID
        {
            get
            {
                return "LAST_INSERT_ROWID";
            }
        }

        public override string CONCAT
        {
            get
            {
                return "||";
            }
        }

        public override string BATCH
        {
            get
            {
                return string.Format("{0};", Environment.NewLine);
            }
        }

        public string ROW_NUMBER
        {
            get
            {
                return "ROW_NUMBER";
            }
        }

        public string OVER
        {
            get
            {
                return "OVER";
            }
        }
    }
}
