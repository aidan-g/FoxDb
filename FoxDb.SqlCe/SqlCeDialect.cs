using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlCeQueryDialect : SqlQueryDialect
    {
        public SqlCeQueryDialect(IDatabase database)
            : base(database)
        {

        }

        public override IDatabaseQueryTypes Types
        {
            get
            {
                return new SqlCeQueryTypes(this.Database);
            }
        }

        public string TOP
        {
            get
            {
                return "TOP";
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
