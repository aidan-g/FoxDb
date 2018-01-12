using System;

namespace FoxDb
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RelationAttribute : Attribute
    {
        public RelationAttribute()
        {
            this.Flags = Defaults.Relation.Flags;
        }

        public RelationAttribute(RelationFlags flags) : this()
        {
            this.Flags |= flags;
        }

        public RelationFlags Flags { get; set; }
    }
}
