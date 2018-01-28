using FoxDb.Interfaces;

namespace FoxDb
{
    public static partial class Extensions
    {
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
