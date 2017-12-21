using System.Collections.Generic;

namespace FoxDb.Templates
{
    public partial class Update
    {
        public Update(string table, string key, IEnumerable<string> fields)
        {
            this.Table = table;
            this.Key = key;
            this.Fields = fields;
        }

        public string Table { get; private set; }

        public string Key { get; private set; }

        public IEnumerable<string> Fields { get; private set; }
    }
}
