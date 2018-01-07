using FoxDb.Interfaces;

namespace FoxDb
{
    public static class Defaults
    {
        static Defaults()
        {
            DefaultColumns = true;
            DefaultRelations = true;
            DefaultRelationBehaviour = RelationBehaviour.EagerFetch;
        }

        public static bool DefaultColumns { get; set; }

        public static bool DefaultRelations { get; set; }

        public static RelationBehaviour DefaultRelationBehaviour { get; set; }
    }
}
