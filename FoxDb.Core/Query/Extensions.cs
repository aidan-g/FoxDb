using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static IQueryGraphBuilder SelectByRelation(this IDatabase database, IRelationConfig relation)
        {
            var builder = database.QueryFactory.Build();
            builder.Select.AddColumns(relation.RightTable.Columns);
            builder.From.AddTable(relation.RightTable);
            switch (relation.Flags.GetMultiplicity())
            {
                case RelationFlags.OneToOne:
                case RelationFlags.OneToMany:
                    builder.Where.AddColumn(relation.RightColumn);
                    break;
                case RelationFlags.ManyToMany:
                    builder.From.AddRelation(relation.Invert());
                    builder.Where.AddColumn(relation.MappingTable.LeftForeignKey);
                    break;
                default:
                    throw new NotImplementedException();
            }

            builder.OrderBy.AddColumns(relation.RightTable.PrimaryKeys);
            return builder;
        }
    }
}
