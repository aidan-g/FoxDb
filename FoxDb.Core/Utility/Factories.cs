using FoxDb.Interfaces;

namespace FoxDb
{
    public static class Factories
    {
        public static ITableFactory Table = new TableFactory();

        public static IColumnFactory Column = new ColumnFactory();

        public static IRelationFactory Relation = new RelationFactory();

        public static ICollectionFactory Collection = new ListCollectionFactory();
    }
}
