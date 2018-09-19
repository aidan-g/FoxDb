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

        public string IDENTITY
        {
            get
            {
                return "@@IDENTITY";
            }
        }

        public string CASE
        {
            get
            {
                return "CASE";
            }
        }

        public string WHEN
        {
            get
            {
                return "WHEN";
            }
        }

        public string THEN
        {
            get
            {
                return "THEN";
            }
        }

        public string ELSE
        {
            get
            {
                return "ELSE";
            }
        }

        public string END
        {
            get
            {
                return "END";
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
    }
}
