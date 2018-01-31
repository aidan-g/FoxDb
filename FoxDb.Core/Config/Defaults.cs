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
                Flags = TableFlags.ValidateSchema | TableFlags.AutoColumns | TableFlags.AutoRelations;
            }

            public static TableFlags Flags { get; set; }
        }

        public static class Column
        {
            static Column()
            {
                Flags = ColumnFlags.ValidateSchema;
            }

            public static ColumnFlags Flags { get; set; }
        }

        public static class Relation
        {
            static Relation()
            {
                Flags = RelationFlags.AutoExpression | RelationFlags.EagerFetch;
            }

            public static RelationFlags Flags { get; set; }
        }

        public static class Persistence
        {
            static Persistence()
            {
                Flags = PersistenceFlags.Cascade;
            }

            public static PersistenceFlags Flags { get; set; }
        }
    }

    [Flags]
    public enum TableFlags : byte
    {
        None = 0,
        ValidateSchema = 1,
        AutoColumns = 2,
        AutoRelations = 4,
        Transient = 8,
        Extern = 16
    }

    [Flags]
    public enum ColumnFlags : byte
    {
        None = 0,
        ValidateSchema = 1
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
        AutoExpression = 32,
        EagerFetch = 64,
    }

    [Flags]
    public enum PersistenceFlags
    {
        None = 0,
        AddOrUpdate = 1,
        Delete = 2,
        Cascade = 4
    }
}
