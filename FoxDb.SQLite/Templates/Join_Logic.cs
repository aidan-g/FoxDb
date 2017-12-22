using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb.Templates
{
    public partial class Join
    {
        public Join(string table1, string table2, string column1, string column2, params IDatabaseQueryCriteria[] criteria)
        {
            this.Table1 = table1;
            this.Table2 = table2;
            this.Column1 = column1;
            this.Column2 = column2;
            this.Criteria = criteria;
        }

        public string Table1 { get; private set; }

        public string Table2 { get; private set; }

        public string Column1 { get; private set; }

        public string Column2 { get; private set; }

        public IEnumerable<IDatabaseQueryCriteria> Criteria { get; private set; }
    }
}
