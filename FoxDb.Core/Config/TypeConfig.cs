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

        public override string ToString()
        {
            return Enum.GetName(typeof(DbType), this.Type);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            unchecked
            {
                hashCode += this.Type.GetHashCode();
                hashCode += this.Size.GetHashCode();
                hashCode += this.Precision.GetHashCode();
                hashCode += this.Scale.GetHashCode();
                hashCode += this.IsNullable.GetHashCode();
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is ITypeConfig)
            {
                return this.Equals(obj as ITypeConfig);
            }
            return base.Equals(obj);
        }

        public bool Equals(ITypeConfig other)
        {
            if (other == null)
            {
                return false;
            }
            if (this.Type != other.Type)
            {
                return false;
            }
            if (this.Size != other.Size)
            {
                return false;
            }
            if (this.Precision != other.Precision)
            {
                return false;
            }
            if (this.Scale != other.Scale)
            {
                return false;
            }
            if (this.IsNullable != other.IsNullable)
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(TypeConfig a, TypeConfig b)
        {
            if ((object)a == null && (object)b == null)
            {
                return true;
            }
            if ((object)a == null || (object)b == null)
            {
                return false;
            }
            if (object.ReferenceEquals((object)a, (object)b))
            {
                return true;
            }
            return a.Equals(b);
        }

        public static bool operator !=(TypeConfig a, TypeConfig b)
        {
            return !(a == b);
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
