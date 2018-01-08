using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SQLiteSchema : IDatabaseSchema
    {
        public SQLiteSchema(IDatabase database)
        {
            this.Database = database;
        }

        public IDatabase Database { get; private set; }

        protected IEnumerable<string> TableNames { get; set; }

        protected IDictionary<string, IEnumerable<string>> ColumnNames { get; set; }

        public bool TableExists(string tableName)
        {
            return this.GetTableNames().Contains(tableName, StringComparer.OrdinalIgnoreCase);
        }

        public bool ColumnExists(string tableName, string columnName)
        {
            return this.GetColumnNames(tableName).Contains(columnName, StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<string> GetTableNames()
        {
            if (this.TableNames == null)
            {
                var query = new DatabaseQuery("SELECT name FROM sqlite_master WHERE type = 'table'");
                using (var reader = this.Database.ExecuteReader(query))
                {
                    this.TableNames = reader.Select(element => element.Get<string>("name")).ToArray();
                }
            }
            return this.TableNames;
        }

        public IEnumerable<string> GetColumnNames(string tableName)
        {
            if (this.ColumnNames == null)
            {
                this.ColumnNames = new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase);
            }
            if (!this.ColumnNames.ContainsKey(tableName))
            {
                var query = new DatabaseQuery(string.Format("PRAGMA table_info('{0}')", tableName));
                using (var reader = this.Database.ExecuteReader(query))
                {
                    this.ColumnNames[tableName] = reader.Select(element => element.Get<string>("name")).ToArray();
                }
            }
            return this.ColumnNames[tableName];
        }
    }
}
