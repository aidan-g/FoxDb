using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static IQueryGraphBuilder FetchByRelation(this IDatabase database, IRelationConfig relation)
        {
            var builder = database.QueryFactory.Build();
            builder.Output.AddColumns(relation.RightTable.Columns);
            builder.Source.AddTable(relation.RightTable);
            switch (relation.Flags.GetMultiplicity())
            {
                case RelationFlags.OneToOne:
                case RelationFlags.OneToMany:
                    builder.Filter.AddColumn(relation.RightColumn);
                    break;
                case RelationFlags.ManyToMany:
                    builder.Source.AddRelation(relation.Invert());
                    builder.Filter.AddColumn(relation.MappingTable.LeftForeignKey);
                    break;
                default:
                    throw new NotImplementedException();
            }

            builder.Sort.AddColumns(relation.RightTable.PrimaryKeys);
            return builder;
        }
    }
}
