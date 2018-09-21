using System;
using System.Data;

namespace FoxDb
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute()
        {
            this.Flags = Defaults.Column.Flags;
        }

        public ColumnAttribute(ColumnFlags flags)
            : this()
        {
            this.Flags |= flags;
        }

        public string Name { get; set; }

        public string Identifier { get; set; }

        public DbType? Type { get; set; }

        public int? Size { get; set; }

        public int? Precision { get; set; }

        public int? Scale { get; set; }

        public bool? IsNullable { get; set; }

        public ColumnFlags Flags { get; set; }
    }
}
