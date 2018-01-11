using System;

namespace FoxDb
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute()
        {
            this.Flags = Defaults.Column.Flags;
        }

        public string Name { get; set; }

        public ColumnFlags Flags { get; set; }
    }
}
