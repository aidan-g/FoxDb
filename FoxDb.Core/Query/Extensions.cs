using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static IQueryGraphBuilder SelectByPrimaryKey<T>(this IDatabase database)
        {
            var table = database.Config.Table<T>();
            var builder = database.QueryFactory.Build();
            builder.Select.AddColumns(table.Columns);
            builder.From.AddTable(table);
            builder.Where.AddColumns(table.PrimaryKeys);
            builder.OrderBy.AddColumns(table.PrimaryKeys);
            return builder;
        }

        public static IQueryGraphBuilder SelectByRelation(this IDatabase database, IRelationConfig relation)
        {
            var builder = database.QueryFactory.Build();
            builder.Select.AddColumns(relation.Child.Columns);
            builder.From.AddTable(relation.Child);
            switch (relation.Multiplicity)
            {
                case RelationMultiplicity.OneToOne:
                case RelationMultiplicity.OneToMany:
                    builder.Where.AddColumn(relation.RightColumn);
                    break;
                case RelationMultiplicity.ManyToMany:
                    builder.From.AddRelation(relation.Invert());
                    builder.Where.AddColumn(relation.Intermediate.LeftForeignKey);
                    break;
                default:
                    throw new NotImplementedException();
            }

            builder.OrderBy.AddColumns(relation.Child.PrimaryKeys);
            return builder;
        }
    }
}
