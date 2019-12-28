using FoxDb.Interfaces;
using System;
using System.Data;
using System.Reflection;

namespace FoxDb
{
    public class TypeFactory : ITypeFactory
    {
        public ITypeConfig Create(ITypeSelector selector)
        {
            switch (selector.SelectorType)
            {
                case TypeSelectorType.Details:
                    return this.Create(selector.Type, selector.Size, selector.Precision, selector.Scale, selector.IsNullable);
                case TypeSelectorType.Property:
                    return this.Create(selector.Property);
                default:
                    throw new NotImplementedException();
            }
        }

        public ITypeConfig Create(DbType? type = null, int? size = null, byte? precision = null, byte? scale = null, bool? isNullable = null)
        {
            if (!type.HasValue)
            {
                type = Defaults.Column.Type.Type;
            }
            if (!size.HasValue)
            {
                size = Defaults.Column.Type.Size;
            }
            if (!precision.HasValue)
            {
                precision = Defaults.Column.Type.Precision;
            }
            if (!scale.HasValue)
            {
                scale = Defaults.Column.Type.Scale;
            }
            if (!isNullable.HasValue)
            {
                isNullable = Defaults.Column.Type.IsNullable;
            }
            return new TypeConfig(type.Value, size.Value, precision.Value, scale.Value, isNullable.Value);
        }

        public ITypeConfig Create(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<TypeAttribute>(true) ?? new TypeAttribute();
            if (!attribute.TypeSpecified)
            {
                attribute.Type = TypeHelper.GetDbType(property.PropertyType);
            }
            if (!attribute.IsNullableSpecified)
            {
                attribute.IsNullable = TypeHelper.GetIsNullable(property.PropertyType);
            }
            return new TypeConfig(
                attribute.Type,
                attribute.Size,
                attribute.Precision,
                attribute.Scale,
                attribute.IsNullable
            );
        }
    }
}
