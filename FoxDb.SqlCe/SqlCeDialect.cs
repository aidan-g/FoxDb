namespace FoxDb
{
    public class SqlCeQueryDialect : SqlQueryDialect
    {
        public override string IDENTITY
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
    }
}
