using FoxDb.Interfaces;
using System.Data;
using System.Reflection;

namespace FoxDb
{
    public class TypeSelector : ITypeSelector
    {
        public DbType? Type { get; private set; }

        public int? Size { get; private set; }

        public byte? Precision { get; private set; }

        public byte? Scale { get; private set; }

        public bool? IsNullable { get; private set; }

        public PropertyInfo Property { get; private set; }

        public TypeSelectorType SelectorType { get; private set; }

        public static ITypeSelector By(DbType? type = null, int? size = null, byte? precision = null, byte? scale = null, bool? isNullable = null)
        {
            return new TypeSelector()
            {
                Type = type,
                Size = size,
                Precision = precision,
                Scale = scale,
                IsNullable = isNullable,
                SelectorType = TypeSelectorType.Details
            };
        }

        public static ITypeSelector By(PropertyInfo property)
        {
            return new TypeSelector()
            {
                Property = property,
                SelectorType = TypeSelectorType.Property
            };
        }
    }
}
