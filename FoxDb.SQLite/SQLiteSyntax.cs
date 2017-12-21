using System.Linq;

namespace FoxDb
{
    public static class SQLiteSyntax
    {
        public const string IDENTIFIER_FORMAT = "\"{0}\"";

        public const string IDENTIFIER_SEPARATOR = ".";

        public const string STRING_FORMAT = "'{0}'";

        public static string Identifier(params string[] identifiers)
        {
            return string.Join(
                IDENTIFIER_SEPARATOR,
                identifiers.Select(identifier => string.Format(IDENTIFIER_FORMAT, identifier))
            );
        }

        public static string String(string name)
        {
            return string.Format(STRING_FORMAT, name.Replace("'", "''"));
        }
    }
}
