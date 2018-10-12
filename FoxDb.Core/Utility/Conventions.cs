using FoxDb.Interfaces;
using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FoxDb
{
    public static class Conventions
    {
        public static PluralizationService Pluralization = PluralizationService.CreateService(CultureInfo.CurrentCulture);

        public static Func<Type, string> TableName = type => Pluralization.Pluralize(type.Name);

        public static Func<ITableConfig, ITableConfig, string> RelationTableName = (table1, table2) => string.Format("{0}_{1}", Pluralization.Singularize(table1.TableName), Pluralization.Singularize(table2.TableName));

        public static string KeyColumn = "Id";

        public static Func<ITableConfig, string> RelationColumn = table => string.Format("{0}_{1}", Pluralization.Singularize(table.TableName), table.PrimaryKey.ColumnName);

        public static Func<PropertyInfo, string> ColumnName = property => property.Name;

        public static Func<IColumnConfig, string> ParameterName = column => Regex.Replace(column.ColumnName, "[^a-z0-9]", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static Func<ITableConfig, ITableConfig, string> RelationName = (leftTable, rightTable) => string.Format("FK_{0}_{1}", leftTable.TableName, rightTable.TableName);

        public static Func<IIndexConfig, string> IndexName = index => string.Format("IX_{0}_{1}", index.Table.TableName, string.Join("_", index.Columns.Select(column => column.ColumnName)));
    }
}
