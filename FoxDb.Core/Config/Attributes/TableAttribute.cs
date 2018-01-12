using System;

namespace FoxDb
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TableAttribute : Attribute
    {
        public TableAttribute()
        {
            this.Flags = Defaults.Table.Flags;
        }

        public TableAttribute(TableFlags flags) : this()
        {
            this.Flags |= flags;
        }

        public string Name { get; set; }

        public TableFlags Flags { get; set; }
    }
}
