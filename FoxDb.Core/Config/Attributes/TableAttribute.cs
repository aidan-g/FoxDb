using System;

namespace FoxDb
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }

        public string Identifier { get; set; }

        private TableFlags? _Flags { get; set; }

        public bool IsFlagsSpecified
        {
            get
            {
                return this._Flags.HasValue;
            }
        }

        public TableFlags Flags
        {
            get
            {
                return this._Flags.HasValue ? this._Flags.Value : Defaults.Table.Flags;
            }
            set
            {
                this._Flags = value;
            }
        }
    }
}
