using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class TableSelector : ITableSelector
    {
        public Type TableType { get; private set; }

        public ITableConfig LeftTable { get; private set; }

        public ITableConfig RightTable { get; private set; }

        public TableFlags Flags { get; private set; }

        public TableSelectorType Type { get; private set; }

        public static ITableSelector By(Type tableType, TableFlags flags)
        {
            return new TableSelector()
            {
                TableType = tableType,
                Flags = flags,
                Type = TableSelectorType.TableType
            };
        }

        public static ITableSelector By(ITableConfig leftTable, ITableConfig rightTable, TableFlags flags)
        {
            return new TableSelector()
            {
                LeftTable = leftTable,
                RightTable = rightTable,
                Flags = flags,
                Type = TableSelectorType.Mapping
            };
        }
    }
}
