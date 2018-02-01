namespace FoxDb
{
    public class SqlCeQueryDialect : SqlQueryDialect
    {
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
    }
}
