using FoxDb.Interfaces;

namespace FoxDb
{
    public abstract class SqlQueryDialect : IDatabaseQueryDialect
    {
        protected SqlQueryDialect(IDatabase database)
        {
            this.Database = database;
        }

        public IDatabase Database { get; private set; }

        public virtual IDatabaseQueryTypes Types
        {
            get
            {
                return new SqlQueryTypes(this.Database);
            }
        }

        public virtual string SELECT
        {
            get
            {
                return "SELECT";
            }
        }

        public virtual string INSERT
        {
            get
            {
                return "INSERT INTO";
            }
        }

        public virtual string VALUES
        {
            get
            {
                return "VALUES";
            }
        }

        public virtual string UPDATE
        {
            get
            {
                return "UPDATE";
            }
        }

        public virtual string SET
        {
            get
            {
                return "SET";
            }
        }

        public virtual string DELETE
        {
            get
            {
                return "DELETE";
            }
        }

        public virtual string FROM
        {
            get
            {
                return "FROM";
            }
        }

        public virtual string JOIN
        {
            get
            {
                return "LEFT JOIN";
            }
        }

        public virtual string ON
        {
            get
            {
                return "ON";
            }
        }

        public virtual string WHERE
        {
            get
            {
                return "WHERE";
            }
        }

        public virtual string ORDER_BY
        {
            get
            {
                return "ORDER BY";
            }
        }

        public virtual string GROUP_BY
        {
            get
            {
                return "GROUP BY";
            }
        }

        public abstract string LAST_INSERT_ID { get; }

        public virtual string AND
        {
            get
            {
                return "&";
            }
        }

        public virtual string AND_ALSO
        {
            get
            {
                return "AND";
            }
        }

        public virtual string OR
        {
            get
            {
                return "|";
            }
        }

        public virtual string OR_ELSE
        {
            get
            {
                return "OR";
            }
        }

        public virtual string COUNT
        {
            get
            {
                return "COUNT";
            }
        }

        public virtual string EXISTS
        {
            get
            {
                return "EXISTS";
            }
        }

        public virtual string STAR
        {
            get
            {
                return "*";
            }
        }

        public abstract string CONCAT { get; }

        public virtual string NULL
        {
            get
            {
                return "NULL";
            }
        }

        public virtual string AS
        {
            get
            {
                return "AS";
            }
        }

        public virtual string ASC
        {
            get
            {
                return "ASC";
            }
        }

        public virtual string DESC
        {
            get
            {
                return "DESC";
            }
        }

        public virtual string LIMIT
        {
            get
            {
                return "LIMIT";
            }
        }

        public virtual string OFFSET
        {
            get
            {
                return "OFFSET";
            }
        }

        public virtual string DISTINCT
        {
            get
            {
                return "DISTINCT";
            }
        }

        public virtual string LIST_DELIMITER
        {
            get
            {
                return ",";
            }
        }

        public virtual string IDENTIFIER_DELIMITER
        {
            get
            {
                return ".";
            }
        }

        public virtual string PARAMETER
        {
            get
            {
                return "@";
            }
        }

        public virtual string NOT
        {
            get
            {
                return "NOT";
            }
        }

        public virtual string IS
        {
            get
            {
                return "IS";
            }
        }

        public virtual string IN
        {
            get
            {
                return "IN";
            }
        }

        public virtual string EQUAL
        {
            get
            {
                return "=";
            }
        }

        public virtual string NOT_EQUAL
        {
            get
            {
                return "<>";
            }
        }

        public virtual string GREATER
        {
            get
            {
                return ">";
            }
        }

        public virtual string GREATER_OR_EQUAL
        {
            get
            {
                return ">=";
            }
        }

        public virtual string LESS
        {
            get
            {
                return "<";
            }
        }

        public virtual string LESS_OR_EQUAL
        {
            get
            {
                return "<=";
            }
        }

        public virtual string LIKE
        {
            get
            {
                return "LIKE";
            }
        }

        public virtual string OPEN_PARENTHESES
        {
            get
            {
                return "(";
            }
        }

        public virtual string CLOSE_PARENTHESES
        {
            get
            {
                return ")";
            }
        }

        public virtual string BETWEEN
        {
            get
            {
                return "BETWEEN";
            }
        }

        public virtual string PLUS
        {
            get
            {
                return "+";
            }
        }

        public virtual string MINUS
        {
            get
            {
                return "-";
            }
        }

        public virtual string ADD
        {
            get
            {
                return "ADD";
            }
        }

        public virtual string DEFAULT
        {
            get
            {
                return "DEFAULT";
            }
        }

        public virtual string IDENTIFIER_FORMAT
        {
            get
            {
                return "\"{0}\"";
            }
        }

        public virtual string STRING_FORMAT
        {
            get
            {
                return "'{0}'";
            }
        }

        public abstract string BATCH { get; }

        public virtual string TABLE
        {
            get
            {
                return "TABLE";
            }
        }

        public virtual string INDEX
        {
            get
            {
                return "INDEX";
            }
        }

        public virtual string CREATE
        {
            get
            {
                return "CREATE";
            }
        }

        public virtual string ALTER
        {
            get
            {
                return "ALTER";
            }
        }

        public virtual string DROP
        {
            get
            {
                return "DROP";
            }
        }

        public virtual string UNIQUE
        {
            get
            {
                return "UNIQUE";
            }
        }

        public virtual string PRIMARY_KEY
        {
            get
            {
                return "PRIMARY KEY";
            }
        }

        public virtual string FOREIGN_KEY
        {
            get
            {
                return "FOREIGN KEY";
            }
        }

        public virtual string CASE
        {
            get
            {
                return "CASE";
            }
        }

        public virtual string WHEN
        {
            get
            {
                return "WHEN";
            }
        }

        public virtual string THEN
        {
            get
            {
                return "THEN";
            }
        }

        public virtual string ELSE
        {
            get
            {
                return "ELSE";
            }
        }

        public virtual string END
        {
            get
            {
                return "END";
            }
        }

        public string CONSTRAINT
        {
            get
            {
                return "CONSTRAINT";
            }
        }

        public string REFERENCES
        {
            get
            {
                return "REFERENCES";
            }
        }

        public string WITH
        {
            get
            {
                return "WITH";
            }
        }

        public string UNION
        {
            get
            {
                return "UNION";
            }
        }

        public string ALL
        {
            get
            {
                return "ALL";
            }
        }
    }
}
