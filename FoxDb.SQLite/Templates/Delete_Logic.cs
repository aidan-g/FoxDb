using System.Collections.Generic;

namespace FoxDb.Templates
{
    public partial class Delete
    {
        public Delete(string table, params string[] keys)
        {
            this.Table = table;
            this.Keys = keys;
        }

        public string Table { get; private set; }

        public IEnumerable<string> Keys { get; private set; }
    }
}
