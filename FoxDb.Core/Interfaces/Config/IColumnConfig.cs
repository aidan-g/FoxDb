using System;

namespace FoxDb.Interfaces
{
    public interface IColumnConfig
    {
        ITableConfig Table { get; }

        string ColumnName { get; set; }

        string PropertyName { get; set; }

        Type PropertyType { get; set; }

        bool IsPrimaryKey { get; set; }

        bool IsForeignKey { get; set; }

        Func<object, object> Getter { get; set; }

        Action<object, object> Setter { get; set; }
    }
}
