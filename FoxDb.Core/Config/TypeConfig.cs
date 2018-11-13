using FoxDb.Interfaces;
using System;
using System.Data;
using System.Reflection;

namespace FoxDb
{
    public class TypeConfig : ITypeConfig
    {
        public TypeConfig(DbType type, int size, byte precision, byte scale, bool isNullable)
        {
            this.Type = type;
            this.Size = size;
            this.Precision = precision;
            this.Scale = scale;
            this.IsNullable = isNullable;
        }

        public DbType Type { get; set; }

        public int Size { get; set; }

        public byte Precision { get; set; }

        public byte Scale { get; set; }

        public bool IsNullable { get; set; }

        public bool IsNumeric
        {
            get
            {
                return TypeHelper.IsNumeric(this);
            }
        }

        public object DefaultValue
        {
            get
            {
                return TypeHelper.GetDefaultValue(this);
            }
        }

        public ITypeConfig Clone()
        {
            return Factories.Type.Create(
                TypeConfig.By(
                    this.Type,
                    this.Size,
                    this.Precision,
                    this.Scale,
                    this.IsNullable
                )
            );
        }

        public static ITypeConfig Unknown
        {
            get
            {
                return new TypeConfig(DbType.Object, 0, 0, 0, false);
            }
        }

        public static ITypeSelector By(DbType? type = null, int? size = null, byte? precision = null, byte? scale = null, bool? isNullable = null)
        {
            return TypeSelector.By(type, size, precision, scale, isNullable);
        }

        public static ITypeSelector By(PropertyInfo property)
        {
            return TypeSelector.By(property);
        }
    }
}
