using System;
using System.Reflection;

namespace FoxDb.Interfaces
{
    public interface IColumnConfig
    {
        ITableConfig Table { get; }

        string Identifier { get; }

        string ColumnName { get; set; }

        PropertyInfo Property { get; set; }

        bool IsPrimaryKey { get; set; }

        bool IsForeignKey { get; set; }

        Func<object, object> Getter { get; set; }

        Action<object, object> Setter { get; set; }
    }
}
