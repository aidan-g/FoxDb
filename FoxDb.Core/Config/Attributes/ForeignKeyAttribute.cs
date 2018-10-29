using System;

namespace FoxDb
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ForeignKeyAttribute : Attribute
    {
        public ForeignKeyAttribute()
        {
            this.Flags = Defaults.Relation.Flags;
        }

        public ForeignKeyAttribute(RelationFlags flags)
            : this()
        {
            this.Flags |= flags;
        }

        public string Name { get; set; }

        public string Identifier { get; set; }

        public Type TableType { get; set; }

        public RelationFlags Flags { get; set; }
    }
}
