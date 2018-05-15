using FoxDb.Interfaces;
using System.Reflection;

namespace FoxDb
{
    public class TypeSelector : ITypeSelector
    {
        public PropertyInfo Property { get; private set; }

        public TypeSelectorType Type { get; private set; }

        public static ITypeSelector By(PropertyInfo property)
        {
            return new TypeSelector()
            {
                Property = property,
                Type = TypeSelectorType.Property
            };
        }
    }
}
