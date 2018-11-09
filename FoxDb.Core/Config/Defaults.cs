using FoxDb.Interfaces;
using System;
using System.Data;

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
                Flags = TableFlags.ValidateSchema | TableFlags.AutoColumns | TableFlags.AutoIndexes | TableFlags.AutoRelations;
            }

            public static TableFlags Flags { get; set; }
        }

        public static class Column
        {
            static Column()
            {
                Type = new TypeConfig(DbType.AnsiString, 0, 0, 0, false);
                Flags = ColumnFlags.ValidateSchema;
            }

            public static ITypeConfig Type { get; set; }

            public static ColumnFlags Flags { get; set; }
        }

        public static class PrimaryKey
        {
            static PrimaryKey()
            {
                Flags = ColumnFlags.Generated;
            }

            public static ColumnFlags Flags { get; set; }
        }

        public static class ForeignKey
        {
            static ForeignKey()
            {
                Flags = ColumnFlags.None;
            }

            public static ColumnFlags Flags { get; set; }
        }

        public static class Index
        {
            static Index()
            {
                Name = "DEFAULT";
                Flags = IndexFlags.None;
            }

            public static string Name { get; set; }

            public static IndexFlags Flags { get; set; }
        }

        public static class Relation
        {
            static Relation()
            {
                Flags = RelationFlags.AutoExpression | RelationFlags.EagerFetch | RelationFlags.AllowNull;
            }

            public static RelationFlags Flags { get; set; }
        }

        public static class Enumerator
        {
            static Enumerator()
            {
                Flags = EnumeratorFlags.None;
            }

            public static EnumeratorFlags Flags { get; set; }
        }
    }

    [Flags]
    public enum TableFlags : byte
    {
        None = 0,
        ValidateSchema = 1,
        AutoColumns = 2,
        AutoIndexes = 4,
        AutoRelations = 8,
        Transient = 16,
        Extern = 32
    }

    [Flags]
    public enum ColumnFlags : byte
    {
        None = 0,
        ValidateSchema = 1,
        Generated = 2,
        ConcurrencyCheck = 4,
        StateCheck = 8
    }

    [Flags]
    public enum IndexFlags : byte
    {
        None = 0,
        Unique = 1
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
        AllowNull = 128
    }

    [Flags]
    public enum EnumeratorFlags
    {
        None = 0
    }
}
