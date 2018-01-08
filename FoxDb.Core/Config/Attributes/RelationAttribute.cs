using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RelationAttribute : Attribute
    {
        public RelationAttribute()
        {
            this.DefaultColumns = Defaults.Relation.DefaultColumns;
            this.Behaviour = Defaults.Relation.DefaultBehaviour;
            this.Multiplicity = Defaults.Relation.DefaultMultiplicity;
        }

        public bool DefaultColumns { get; set; }

        public RelationBehaviour Behaviour { get; set; }

        public RelationMultiplicity Multiplicity { get; set; }
    }
}
