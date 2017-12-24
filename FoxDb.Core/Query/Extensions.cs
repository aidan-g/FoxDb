using FoxDb.Interfaces;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static IDatabaseQuery SelectByPrimaryKey<T>(this IDatabase database) where T : IPersistable
        {
            var table = database.Config.Table<T>();
            var composer = database.QueryFactory.Compose()
                .Select()
                .Table(table)
                .IdentifierDelimiter()
                .Star()
                .From()
                .Table(table)
                .Where()
                .AssignParametersToColumns(table.PrimaryKeys);
            return composer.Query;
        }

        public static IDatabaseQuery SelectByRelation<T>(this IDatabase database, IRelationConfig relation) where T : IPersistable
        {
            var table = database.Config.Table<T>();
            var composer = database.QueryFactory.Compose()
                .Select()
                .Table(table)
                .IdentifierDelimiter()
                .Star()
                .From()
                .Table(table)
                .Where()
                .AssignParameterToColumn(relation.Column);
            return composer.Query;
        }

        public static IDatabaseQuery SelectByRelation<T1, T2>(this IDatabase database, IRelationConfig relation) where T1 : IPersistable where T2 : IPersistable
        {
            var table1 = database.Config.Table<T2>();
            var table2 = database.Config.Table<T1, T2>();
            var composer = database.QueryFactory.Compose()
                .Select()
                .Table(table1)
                .IdentifierDelimiter()
                .Star()
                .From()
                .Table(table1)
                .Join()
                .Table(table2)
                .On()
                .Column(table2.RightForeignKey)
                .Operator(QueryOperator.Equals)
                .Column(table1.PrimaryKey)
                .Where()
                .AssignParameterToColumn(table2.LeftForeignKey);
            return composer.Query;
        }
    }
}
