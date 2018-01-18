using FoxDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SQLiteSchema : IDatabaseSchema
    {
        private SQLiteSchema()
        {
            this.ColumnNames = new ConcurrentDictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        }

        public SQLiteSchema(IDatabase database) : this()
        {
            this.Database = database;
        }

        public IDatabase Database { get; private set; }

        protected IEnumerable<string> TableNames { get; set; }

        protected ConcurrentDictionary<string, string[]> ColumnNames { get; set; }

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
            var columnNames = default(string[]);
            if (!this.ColumnNames.TryGetValue(tableName, out columnNames))
            {
                var query = new DatabaseQuery(string.Format("PRAGMA table_info('{0}')", tableName));
                using (var reader = this.Database.ExecuteReader(query))
                {
                    columnNames = reader.Select(element => element.Get<string>("name")).ToArray();
                    if (columnNames.Length == 0)
                    {
                        throw new InvalidOperationException(string.Format("No columns were found for table \"{0}\".", tableName));
                    }
                    if (!this.ColumnNames.TryAdd(tableName, columnNames))
                    {
                        //TODO: Warn?
                    }
                }
            }
            return columnNames;
        }
    }
}
