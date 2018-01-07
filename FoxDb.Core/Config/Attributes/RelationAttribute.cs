using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RelationAttribute : Attribute
    {
        public RelationAttribute()
        {
            this.DefaultColumns = true;
            this.Behaviour = RelationBehaviour.EagerFetch;
            this.Multiplicity = RelationMultiplicity.None;
        }

        public bool DefaultColumns { get; set; }

        public RelationBehaviour Behaviour { get; set; }

        public RelationMultiplicity Multiplicity { get; set; }
    }
}
