using System;

namespace FoxDb
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IndexAttribute : Attribute
    {
        public IndexAttribute()
        {
            this.Name = Defaults.Index.Name;
        }

        public string Name { get; set; }

        public string Identifier { get; set; }

        public IndexFlags? _Flags { get; set; }

        public bool IsFlagsSpecified
        {
            get
            {
                return this._Flags.HasValue;
            }
        }

        public IndexFlags Flags
        {
            get
            {
                return this._Flags.HasValue ? this._Flags.Value : Defaults.Index.Flags;
            }
            set
            {
                this._Flags = value;
            }
        }
    }
}
