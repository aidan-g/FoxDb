using System;

namespace FoxDb
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TableAttribute : Attribute
    {
        public TableAttribute()
        {

        }

        public string Name { get; set; }

        public bool DefaultColumns { get; set; }

        public bool DefaultRelations { get; set; }
    }
}
