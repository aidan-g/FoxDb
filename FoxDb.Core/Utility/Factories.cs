using FoxDb.Interfaces;

namespace FoxDb
{
    public static class Factories
    {
        public static ITableFactory Table = new TableFactory();

        public static IColumnFactory Column = new ColumnFactory();

        public static IRelationFactory Relation = new RelationFactory();

        public static ICollectionFactory Collection = new CollectionFactory();

        public static ITypeFactory Type = new TypeFactory();

        public static class PropertyAccessor
        {
            public static IPropertyAccessorFactory Column = new PropertyAccessorFactory(true);

            public static IPropertyAccessorFactory Relation = new PropertyAccessorFactory(false);
        }
    }
}
