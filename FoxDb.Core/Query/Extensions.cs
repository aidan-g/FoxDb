using FoxDb.Interfaces;
using System.Linq;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static string Identifier(this IDatabaseQueryDialect dialect, params string[] identifiers)
        {
            return string.Join(
                dialect.IDENTIFIER_DELIMITER,
                identifiers.Select(
                    identifier => string.Format(dialect.IDENTIFIER_FORMAT, identifier)
                )
            );
        }

        public static string String(this IDatabaseQueryDialect dialect, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("'", "''");
            }
            return string.Format(
                dialect.STRING_FORMAT,
                value
            );
        }
    }
}
