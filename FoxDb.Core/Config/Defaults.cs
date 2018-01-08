using FoxDb.Interfaces;

namespace FoxDb
{
    public static class Defaults
    {
        static Defaults()
        {
            //Nothing to do.
        }

        public static class Table
        {
            static Table()
            {
                DefaultColumns = true;
                DefaultRelations = true;
            }

            public static bool DefaultColumns { get; set; }

            public static bool DefaultRelations { get; set; }
        }

        public static class Column
        {
            static Column()
            {
                //Nothing to do.
            }
        }

        public static class Relation
        {
            static Relation()
            {
                DefaultColumns = true;
                DefaultMultiplicity = RelationMultiplicity.OneToMany;
                DefaultBehaviour = RelationBehaviour.EagerFetch;
            }

            public static bool DefaultColumns { get; set; }

            public static RelationMultiplicity DefaultMultiplicity { get; set; }

            public static RelationBehaviour DefaultBehaviour { get; set; }
        }
    }
}
