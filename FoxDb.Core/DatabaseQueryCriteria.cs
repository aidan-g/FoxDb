using FoxDb.Interfaces;

namespace FoxDb
{
    public class DatabaseQueryCriteria : IDatabaseQueryCriteria
    {
        public DatabaseQueryCriteria(string table, string column, string @operator = EQUALS)
        {
            this.Table = table;
            this.Column = column;
            this.Operator = @operator;
        }

        public string Table { get; set; }

        public string Column { get; set; }

        public string Operator { get; set; }

        public const string EQUALS = "=";
    }
}
