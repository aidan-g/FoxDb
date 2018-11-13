using System;
using System.Data;

namespace FoxDb
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TypeAttribute : Attribute
    {
        private DbType? _Type { get; set; }

        public bool TypeSpecified
        {
            get
            {
                return this._Type.HasValue;
            }
        }

        public DbType Type
        {
            get
            {
                return this._Type.HasValue ? this._Type.Value : Defaults.Column.Type.Type;
            }
            set
            {
                this._Type = value;
            }
        }

        private int? _Size { get; set; }

        public bool SizeSpecified
        {
            get
            {
                return this._Size.HasValue;
            }
        }

        public int Size
        {
            get
            {
                return this._Size.HasValue ? this._Size.Value : Defaults.Column.Type.Size;
            }
            set
            {
                this._Size = value;
            }
        }

        private byte? _Precision { get; set; }

        public bool PrecisionSpecified
        {
            get
            {
                return this._Precision.HasValue;
            }
        }

        public byte Precision
        {
            get
            {
                return this._Precision.HasValue ? this._Precision.Value : Defaults.Column.Type.Precision;
            }
            set
            {
                this._Precision = value;
            }
        }

        private byte? _Scale { get; set; }

        public bool ScaleSpecified
        {
            get
            {
                return this._Scale.HasValue;
            }
        }

        public byte Scale
        {
            get
            {
                return this._Scale.HasValue ? this._Scale.Value : Defaults.Column.Type.Scale;
            }
            set
            {
                this._Scale = value;
            }
        }

        private bool? _IsNullable { get; set; }

        public bool IsNullableSpecified
        {
            get
            {
                return this._IsNullable.HasValue;
            }
        }

        public bool IsNullable
        {
            get
            {
                return this._IsNullable.HasValue ? this._IsNullable.Value : Defaults.Column.Type.IsNullable;
            }
            set
            {
                this._IsNullable = value;
            }
        }
    }
}
