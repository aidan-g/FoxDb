using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb.Templates
{
    public partial class Select
    {
        public Select(string table, params IDatabaseQueryCriteria[] criteria)
        {
            this.Table = table;
            this.Criteria = criteria;
        }

        public string Table { get; private set; }

        public IEnumerable<IDatabaseQueryCriteria> Criteria { get; private set; }
    }
}
