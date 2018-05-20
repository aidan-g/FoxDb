using FoxDb.Interfaces;
using System.Data;
using System.Reflection;

namespace FoxDb
{
    public class TypeConfig : ITypeConfig
    {
        public TypeConfig(ColumnAttribute attribute)
            : this(attribute.Type, attribute.Size, attribute.Precision, attribute.Scale)
        {

        }

        public TypeConfig(DbType type, int size = 0, int precision = 0, int scale = 0)
        {
            this.Type = type;
            this.Size = size;
            this.Precision = precision;
            this.Scale = scale;
        }

        public DbType Type { get; set; }

        public int Size { get; set; }

        public int Precision { get; set; }

        public int Scale { get; set; }

        public bool IsNullable { get; set; }

        public static ITypeSelector By(PropertyInfo property)
        {
            return TypeSelector.By(property);
        }
    }
}
