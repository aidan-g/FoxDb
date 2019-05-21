namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryDialect
    {
        IDatabaseQueryTypes Types { get; }

        string SELECT { get; }

        string INSERT { get; }

        string VALUES { get; }

        string UPDATE { get; }

        string SET { get; }

        string DELETE { get; }

        string FROM { get; }

        string JOIN { get; }

        string ON { get; }

        string WHERE { get; }

        string ORDER_BY { get; }

        string GROUP_BY { get; }

        string LAST_INSERT_ID { get; }

        string AND { get; }

        string AND_ALSO { get; }

        string OR { get; }

        string OR_ELSE { get; }

        string COUNT { get; }

        string EXISTS { get; }

        string STAR { get; }

        string CONCAT { get; }

        string NULL { get; }

        string AS { get; }

        string ASC { get; }

        string DESC { get; }

        string DISTINCT { get; }

        string LIST_DELIMITER { get; }

        string IDENTIFIER_DELIMITER { get; }

        string PARAMETER { get; }

        string NOT { get; }

        string IS { get; }

        string IN { get; }

        string EQUAL { get; }

        string NOT_EQUAL { get; }

        string GREATER { get; }

        string GREATER_OR_EQUAL { get; }

        string LESS { get; }

        string LESS_OR_EQUAL { get; }

        string LIKE { get; }

        string OPEN_PARENTHESES { get; }

        string CLOSE_PARENTHESES { get; }

        string BETWEEN { get; }

        string PLUS { get; }

        string MINUS { get; }

        string ADD { get; }

        string DEFAULT { get; }

        string IDENTIFIER_FORMAT { get; }

        string STRING_FORMAT { get; }

        string BATCH { get; }

        string TABLE { get; }

        string INDEX { get; }

        string CREATE { get; }

        string ALTER { get; }

        string DROP { get; }

        string UNIQUE { get; }

        string PRIMARY_KEY { get; }

        string FOREIGN_KEY { get; }

        string CASE { get; }

        string WHEN { get; }

        string THEN { get; }

        string ELSE { get; }

        string END { get; }

        string CONSTRAINT { get; }

        string REFERENCES { get; }

        string WITH { get; }

        string UNION { get; }

        string ALL { get; }
    }
}
