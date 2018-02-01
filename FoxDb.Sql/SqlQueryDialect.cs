using FoxDb.Interfaces;

namespace FoxDb
{
    public abstract class SqlQueryDialect : IDatabaseQueryDialect
    {
        public string SELECT
        {
            get
            {
                return "SELECT";
            }
        }

        public string INSERT
        {
            get
            {
                return "INSERT INTO";
            }
        }

        public string VALUES
        {
            get
            {
                return "VALUES";
            }
        }

        public string UPDATE
        {
            get
            {
                return "UPDATE";
            }
        }

        public string SET
        {
            get
            {
                return "SET";
            }
        }

        public string DELETE
        {
            get
            {
                return "DELETE";
            }
        }

        public string FROM
        {
            get
            {
                return "FROM";
            }
        }

        public string JOIN
        {
            get
            {
                return "LEFT JOIN";
            }
        }

        public string ON
        {
            get
            {
                return "ON";
            }
        }

        public string WHERE
        {
            get
            {
                return "WHERE";
            }
        }

        public string ORDER_BY
        {
            get
            {
                return "ORDER BY";
            }
        }

        public string GROUP_BY
        {
            get
            {
                return "GROUP BY";
            }
        }

        public string AND
        {
            get
            {
                return "&";
            }
        }

        public string AND_ALSO
        {
            get
            {
                return "AND";
            }
        }

        public string OR
        {
            get
            {
                return "|";
            }
        }

        public string OR_ELSE
        {
            get
            {
                return "OR";
            }
        }

        public string COUNT
        {
            get
            {
                return "COUNT";
            }
        }

        public string EXISTS
        {
            get
            {
                return "EXISTS";
            }
        }

        public string STAR
        {
            get
            {
                return "*";
            }
        }

        public string NULL
        {
            get
            {
                return "NULL";
            }
        }

        public string AS
        {
            get
            {
                return "AS";
            }
        }

        public string ASC
        {
            get
            {
                return "ASC";
            }
        }

        public string DESC
        {
            get
            {
                return "DESC";
            }
        }

        public string LIMIT
        {
            get
            {
                return "LIMIT";
            }
        }

        public string OFFSET
        {
            get
            {
                return "OFFSET";
            }
        }

        public string DISTINCT
        {
            get
            {
                return "DISTINCT";
            }
        }

        public string LIST_DELIMITER
        {
            get
            {
                return ",";
            }
        }

        public string IDENTIFIER_DELIMITER
        {
            get
            {
                return ".";
            }
        }

        public string PARAMETER
        {
            get
            {
                return "@";
            }
        }

        public string NOT
        {
            get
            {
                return "NOT";
            }
        }

        public string EQUAL
        {
            get
            {
                return "=";
            }
        }

        public string NOT_EQUAL
        {
            get
            {
                return "<>";
            }
        }

        public string GREATER
        {
            get
            {
                return ">";
            }
        }

        public string GREATER_OR_EQUAL
        {
            get
            {
                return ">=";
            }
        }

        public string LESS
        {
            get
            {
                return "<";
            }
        }

        public string LESS_OR_EQUAL
        {
            get
            {
                return "<=";
            }
        }

        public string OPEN_PARENTHESES
        {
            get
            {
                return "(";
            }
        }

        public string CLOSE_PARENTHESES
        {
            get
            {
                return ")";
            }
        }

        public string ADD
        {
            get
            {
                return "+";
            }
        }

        public string DEFAULT
        {
            get
            {
                return "DEFAULT";
            }
        }

        public string IDENTIFIER_FORMAT
        {
            get
            {
                return "\"{0}\"";
            }
        }

        public string STRING_FORMAT
        {
            get
            {
                return "'{0}'";
            }
        }

        public abstract string BATCH { get; }
    }
}
