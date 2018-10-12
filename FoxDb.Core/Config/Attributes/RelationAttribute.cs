using System;

namespace FoxDb
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
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

        public string Identifier { get; set; }

        public string LeftColumn { get; set; }

        public string RightColumn { get; set; }

        public RelationFlags Flags { get; set; }
    }
}
