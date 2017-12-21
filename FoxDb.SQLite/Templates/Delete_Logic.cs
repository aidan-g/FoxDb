namespace FoxDb.Templates
{
    public partial class Delete
    {
        public Delete(string table, string key)
        {
            this.Table = table;
            this.Key = key;
        }

        public string Table { get; private set; }

        public string Key { get; private set; }
    }
}
