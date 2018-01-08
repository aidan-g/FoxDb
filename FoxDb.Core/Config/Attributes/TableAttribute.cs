using System;

namespace FoxDb
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TableAttribute : Attribute
    {
        public TableAttribute()
        {
            this.DefaultColumns = Defaults.Table.DefaultColumns;
            this.DefaultRelations = Defaults.Table.DefaultRelations;
        }

        public string Name { get; set; }

        public bool DefaultColumns { get; set; }

        public bool DefaultRelations { get; set; }
    }
}
