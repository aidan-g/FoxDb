using System.Linq;

namespace FoxDb
{
    public static class SQLiteSyntax
    {
        public const string SELECT = "SELECT";

        public const string INSERT = "INSERT INTO";

        public const string VALUES = "VALUES";

        public const string UPDATE = "UPDATE";

        public const string SET = "SET";

        public const string DELETE = "DELETE";

        public const string FROM = "FROM";

        public const string JOIN = "LEFT JOIN";

        public const string ON = "ON";

        public const string WHERE = "WHERE";

        public const string ORDER_BY = "ORDER BY";

        public const string AND = "&";

        public const string AND_ALSO = "AND";

        public const string OR = "|";

        public const string OR_ELSE = "OR";

        public const string COUNT = "COUNT";

        public const string EXISTS = "EXISTS";

        public const string STAR = "*";

        public const string NULL = "NULL";

        public const string AS = "AS";

        public const string ASC = "ASC";

        public const string DESC = "DESC";

        public const string LIST_DELIMITER = ",";

        public const string IDENTIFIER_DELIMITER = ".";

        public const string PARAMETER = "@";

        public const string EQUAL = "=";

        public const string NOT_EQUAL = "<>";

        public const string OPEN_PARENTHESES = "(";

        public const string CLOSE_PARENTHESES = ")";

        public const string STATEMENT = ";";

        public const string IDENTITY = "LAST_INSERT_ROWID";

        public const string DEFAULT = "DEFAULT";

        public const string IDENTIFIER_FORMAT = "\"{0}\"";

        public const string STRING_FORMAT = "'{0}'";

        public static string Identifier(params string[] identifiers)
        {
            return string.Join(
                IDENTIFIER_DELIMITER,
                identifiers.Select(identifier => string.Format(IDENTIFIER_FORMAT, identifier))
            );
        }

        public static string String(string name)
        {
            return string.Format(STRING_FORMAT, name.Replace("'", "''"));
        }
    }
}
