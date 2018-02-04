using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class TableSelector : ITableSelector
    {
        public string Identifier { get; private set; }

        public string TableName { get; private set; }

        public Type TableType { get; private set; }

        public ITableConfig LeftTable { get; private set; }

        public ITableConfig RightTable { get; private set; }

        public TableFlags Flags { get; private set; }

        public TableSelectorType Type { get; private set; }

        public static ITableSelector By(string identifier, string tableName, TableFlags flags)
        {
            return new TableSelector()
            {
                Identifier = identifier,
                TableName = tableName,
                Flags = flags,
                Type = TableSelectorType.TableName
            };
        }

        public static ITableSelector By(string identifier, Type tableType, TableFlags flags)
        {
            return new TableSelector()
            {
                Identifier = identifier,
                TableType = tableType,
                Flags = flags,
                Type = TableSelectorType.TableType
            };
        }

        public static ITableSelector By(string identifier, ITableConfig leftTable, ITableConfig rightTable, TableFlags flags)
        {
            return new TableSelector()
            {
                Identifier = identifier,
                LeftTable = leftTable,
                RightTable = rightTable,
                Flags = flags,
                Type = TableSelectorType.Mapping
            };
        }
    }
}
