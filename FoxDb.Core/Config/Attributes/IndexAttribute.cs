using System;

namespace FoxDb
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IndexAttribute : Attribute
    {
        public IndexAttribute()
        {
            this.Name = Defaults.Index.Name;
            this.Flags = Defaults.Index.Flags;
        }

        public IndexAttribute(IndexFlags flags)
            : this()
        {
            this.Flags |= flags;
        }

        public string Name { get; set; }

        public string Identifier { get; set; }

        public IndexFlags Flags { get; set; }
    }
}
