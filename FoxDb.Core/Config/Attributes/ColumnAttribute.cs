using System;

namespace FoxDb
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }

        public string Identifier { get; set; }

        private bool? _IsPrimaryKey { get; set; }

        public bool IsPrimaryKeySpecified
        {
            get
            {
                return this._IsPrimaryKey.HasValue;
            }
        }

        public bool IsPrimaryKey
        {
            get
            {
                return this._IsPrimaryKey.HasValue ? this._IsPrimaryKey.Value : false;
            }
            set
            {
                this._IsPrimaryKey = value;
            }
        }

        private bool? _IsForeignKey { get; set; }

        public bool IsForeignKeySpecified
        {
            get
            {
                return this._IsForeignKey.HasValue;
            }
        }

        public bool IsForeignKey
        {
            get
            {
                return this._IsForeignKey.HasValue ? this._IsForeignKey.Value : false;
            }
            set
            {
                this._IsForeignKey = value;
            }
        }

        private ColumnFlags? _Flags { get; set; }

        public bool IsFlagsSpecified
        {
            get
            {
                return this._Flags.HasValue;
            }
        }

        public ColumnFlags Flags
        {
            get
            {
                return this._Flags.HasValue ? this._Flags.Value : Defaults.Column.Flags;
            }
            set
            {
                this._Flags = value;
            }
        }
    }
}
