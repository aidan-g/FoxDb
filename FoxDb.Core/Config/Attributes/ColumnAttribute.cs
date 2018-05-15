using System;
using System.Data;

namespace FoxDb
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute()
        {
            this.Type = Defaults.Column.Type.Type;
            this.Size = Defaults.Column.Type.Size;
            this.Precision = Defaults.Column.Type.Precision;
            this.Scale = Defaults.Column.Type.Scale;
            this.Flags = Defaults.Column.Flags;
        }

        public ColumnAttribute(ColumnFlags flags)
            : this()
        {
            this.Flags |= flags;
        }

        public string Name { get; set; }

        public string Identifier { get; set; }

        public DbType Type { get; set; }

        public int Size { get; set; }

        public int Precision { get; set; }

        public int Scale { get; set; }

        public ColumnFlags Flags { get; set; }
    }
}
