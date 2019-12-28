using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlServerQueryDialect : SqlQueryDialect
    {
        public SqlServerQueryDialect(IDatabase database)
            : base(database)
        {

        }

        public override IDatabaseQueryTypes Types
        {
            get
            {
                return new SqlServerQueryTypes(this.Database);
            }
        }

        public string TOP
        {
            get
            {
                return "TOP";
            }
        }

        public string PERCENT
        {
            get
            {
                return "PERCENT";
            }
        }

        public override string LAST_INSERT_ID
        {
            get
            {
                return "@@IDENTITY";
            }
        }

        public override string CONCAT
        {
            get
            {
                return "+";
            }
        }

        public string IDENTITY
        {
            get
            {
                return "IDENTITY";
            }
        }

        public override string BATCH
        {
            get
            {
                return string.Format("\nGO");
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

        public string ROWLOCK
        {
            get
            {
                return "ROWLOCK";
            }
        }

        public string PAGLOCK
        {
            get
            {
                return "PAGLOCK";
            }
        }

        public string TABLOCK
        {
            get
            {
                return "TABLOCK";
            }
        }

        public string DBLOCK
        {
            get
            {
                return "DBLOCK";
            }
        }

        public string UPDLOCK
        {
            get
            {
                return "UPDLOCK";
            }
        }

        public string XLOCK
        {
            get
            {
                return "XLOCK";
            }
        }

        public string HOLDLOCK
        {
            get
            {
                return "HOLDLOCK";
            }
        }

        public string NOLOCK
        {
            get
            {
                return "NOLOCK";
            }
        }
    }
}
