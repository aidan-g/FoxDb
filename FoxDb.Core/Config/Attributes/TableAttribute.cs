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

        public string Name { get; set; }

        public TableFlags Flags { get; set; }
    }
}
