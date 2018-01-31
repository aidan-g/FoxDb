using FoxDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SqlCeSchema : IDatabaseSchema
    {
        private SqlCeSchema()
        {
            this.ColumnNames = new ConcurrentDictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        }

        public SqlCeSchema(IDatabase database) : this()
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
                var query = new DatabaseQuery("SELECT table_name FROM information_schema.tables WHERE table_type = 'BASE_TABLE'");
                using (var reader = this.Database.ExecuteReader(query))
                {
                    this.TableNames = reader.Select(element => element.Get<string>("table_name")).ToArray();
                }
            }
            return this.TableNames;
        }

        public IEnumerable<string> GetColumnNames(string tableName)
        {
            var columnNames = default(string[]);
            if (!this.ColumnNames.TryGetValue(tableName, out columnNames))
            {
                var query = new DatabaseQuery(string.Format("SELECT column_name FROM information_schema.columns WHERE table_name = '{0}'", tableName));
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
