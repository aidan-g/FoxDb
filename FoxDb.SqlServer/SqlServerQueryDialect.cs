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
