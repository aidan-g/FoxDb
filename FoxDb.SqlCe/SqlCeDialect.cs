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

        public string IDENTITY
        {
            get
            {
                return "@@IDENTITY";
            }
        }

        public override string BATCH
        {
            get
            {
                return string.Format("\nGO");
            }
        }

        public override string PRIMARY_KEY
        {
            get
            {
                return string.Format("IDENTITY {0}", base.PRIMARY_KEY);
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
    }
}
