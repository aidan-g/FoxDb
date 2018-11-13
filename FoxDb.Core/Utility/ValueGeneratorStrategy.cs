using FoxDb.Interfaces;
using System;
using System.Data;

namespace FoxDb
{
    public class ValueGeneratorStrategy : IValueGeneratorStrategy
    {
        public object CreateValue(ITableConfig table, IColumnConfig column, object item)
        {
            switch (column.ColumnType.Type)
            {
                case DbType.Guid:
                    return SequentialGuid.New();
            }
            return column.DefaultValue;
        }

        public static readonly IValueGeneratorStrategy Instance = new ValueGeneratorStrategy();
    }
}
