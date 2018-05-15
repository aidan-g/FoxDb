using System.Reflection;

namespace FoxDb.Interfaces
{
    public interface ITypeSelector
    {
        PropertyInfo Property { get; }

        TypeSelectorType Type { get; }
    }

    public enum TypeSelectorType : byte
    {
        None,
        Property
    }
}
