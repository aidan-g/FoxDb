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
            return string.Format(dialect.STRING_FORMAT, value);
        }

        public static IQueryGraphBuilder FetchByRelation(this IDatabase database, IRelationConfig relation)
        {
            var builder = database.QueryFactory.Build();
            builder.Output.AddColumns(relation.RightTable.Columns);
            builder.Source.AddTable(relation.RightTable);
            builder.RelationManager.AddRelation(relation);
            //TODO: Assuming the relation is using the primary key?
            builder.Filter.AddColumn(relation.LeftTable.PrimaryKey);
            builder.Sort.AddColumns(relation.RightTable.PrimaryKeys);
            return builder;
        }
    }
}
