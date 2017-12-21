using FoxDb.Interfaces;
using FoxDb.Templates;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public static class SQLiteSchema
    {
        public static IEnumerable<string> GetFieldNames(IDatabase database, string table)
        {
            var tableInfo = new TableInfo(table);
            var query = new DatabaseQuery(tableInfo.TransformText());
            using (var reader = database.ExecuteReader(query))
            {
                return reader.Select(element => element["name"]).OfType<string>().ToArray();
            }
        }
    }
}
