using System;

namespace FoxDb
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class RelationAttribute : Attribute
    {
        public string Identifier { get; set; }

        public string LeftColumn { get; set; }

        public string RightColumn { get; set; }

        private RelationFlags? _Flags { get; set; }

        public bool IsFlagsSpecified
        {
            get
            {
                return this._Flags.HasValue;
            }
        }

        public RelationFlags Flags
        {
            get
            {
                return this._Flags.HasValue ? this._Flags.Value : Defaults.Relation.Flags;
            }
            set
            {
                this._Flags = value;
            }
        }
    }
}
