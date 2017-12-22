using System;

namespace FoxDb
{
    public static class Conventions
    {
        public static string KeyColumn = "Id";

        public static Func<Type, string> RelationColumn = type => string.Format("{0}_{1}", type.Name, KeyColumn);
    }
}
