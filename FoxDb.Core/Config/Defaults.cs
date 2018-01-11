using System;

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
                Flags = TableFlags.AutoColumns | TableFlags.AutoRelations;
            }

            public static TableFlags Flags { get; set; }
        }

        public static class Column
        {
            static Column()
            {
                Flags = ColumnFlags.None;
            }

            public static ColumnFlags Flags { get; set; }
        }

        public static class Relation
        {
            static Relation()
            {
                Flags = RelationFlags.AutoColumns | RelationFlags.EagerFetch;
            }

            public static RelationFlags Flags { get; set; }
        }
    }

    [Flags]
    public enum TableFlags : byte
    {
        None = 0,
        AutoColumns = 1,
        AutoRelations = 2,
        Transient = 4
    }

    [Flags]
    public enum ColumnFlags : byte
    {
        None = 0
    }

    [Flags]
    public enum RelationFlags : byte
    {
        None = 0,
        //Multiplicity.
        OneToOne = 1,
        OneToMany = 2,
        ManyToMany = 4,
        //Behaviour.
        AutoColumns = 8,
        EagerFetch = 16
    }
}
