using System;

namespace FoxDb
{
    public static class Conventions
    {
        public static Func<Type, string> TableName = type => Pluralization.Pluralize(type.Name);

        public static Func<Type, Type, string> RelationTableName = (type1, type2) => string.Format("{0}_{1}", Pluralization.Singularize(type1.Name), Pluralization.Singularize(type2.Name));

        public static string KeyColumn = "Id";

        public static Func<Type, string> RelationColumn = type => string.Format("{0}_{1}", type.Name, KeyColumn);
    }
}
