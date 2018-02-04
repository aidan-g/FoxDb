namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryDialect
    {
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

        string AND { get; }

        string AND_ALSO { get; }

        string OR { get; }

        string OR_ELSE { get; }

        string COUNT { get; }

        string EXISTS { get; }

        string STAR { get; }

        string NULL { get; }

        string AS { get; }

        string ASC { get; }

        string DESC { get; }

        string DISTINCT { get; }

        string LIST_DELIMITER { get; }

        string IDENTIFIER_DELIMITER { get; }

        string PARAMETER { get; }

        string NOT { get; }

        string EQUAL { get; }

        string NOT_EQUAL { get; }

        string GREATER { get; }

        string GREATER_OR_EQUAL { get; }

        string LESS { get; }

        string LESS_OR_EQUAL { get; }

        string OPEN_PARENTHESES { get; }

        string CLOSE_PARENTHESES { get; }

        string ADD { get; }

        string DEFAULT { get; }

        string IDENTIFIER_FORMAT { get; }

        string STRING_FORMAT { get; }

        string BATCH { get; }
    }
}
