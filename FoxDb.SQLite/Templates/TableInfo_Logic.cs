namespace FoxDb.Templates
{
    public partial class TableInfo
    {
        public TableInfo(string table)
        {
            this.Table = table;
        }

        public string Table { get; private set; }
    }
}
