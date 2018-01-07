using FoxDb.Interfaces;
using System;
using System.Reflection;

namespace FoxDb
{
    public class ColumnConfig : IColumnConfig
    {
        public ColumnConfig(ITableConfig table, string columnName, PropertyInfo property, Func<object, object> getter, Action<object, object> setter)
        {
            this.Table = table;
            this.ColumnName = columnName;
            this.Property = property;
            this.Getter = getter;
            this.Setter = setter;
        }

        public ITableConfig Table { get; private set; }

        public string ColumnName { get; set; }

        public PropertyInfo Property { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsForeignKey { get; set; }

        public Func<object, object> Getter { get; set; }

        public Action<object, object> Setter { get; set; }
    }
}
