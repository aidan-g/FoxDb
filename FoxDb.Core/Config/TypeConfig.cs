using FoxDb.Interfaces;
using System.Data;
using System.Reflection;

namespace FoxDb
{
    public class TypeConfig : ITypeConfig
    {
        public TypeConfig(DbType type, int size, int precision, int scale, bool isNullable)
        {
            this.Type = type;
            this.Size = size;
            this.Precision = precision;
            this.Scale = scale;
            this.IsNullable = isNullable;
        }

        public DbType Type { get; set; }

        public int Size { get; set; }

        public int Precision { get; set; }

        public int Scale { get; set; }

        public bool IsNullable { get; set; }

        public static ITypeSelector By(DbType? type = null, int? size = null, int? precision = null, int? scale = null, bool? isNullable = null)
        {
            return TypeSelector.By(type, size, precision, scale, isNullable);
        }

        public static ITypeSelector By(PropertyInfo property)
        {
            return TypeSelector.By(property);
        }
    }
}
