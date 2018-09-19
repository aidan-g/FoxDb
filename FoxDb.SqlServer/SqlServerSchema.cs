using FoxDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SqlServerSchema : IDatabaseSchema
    {
        private SqlServerSchema()
        {
            this.ColumnNames = new ConcurrentDictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        }

        public SqlServerSchema(IDatabase database)
            : this()
        {
            this.Database = database;
        }

        public IDatabase Database { get; private set; }

        protected IEnumerable<string> TableNames { get; set; }

        protected ConcurrentDictionary<string, string[]> ColumnNames { get; set; }

        public bool TableExists(string tableName, ITransactionSource transaction = null)
        {
            return this.GetTableNames(transaction).Contains(tableName, StringComparer.OrdinalIgnoreCase);
        }

        public bool ColumnExists(string tableName, string columnName, ITransactionSource transaction = null)
        {
            return this.GetColumnNames(tableName, transaction).Contains(columnName, StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<string> GetTableNames(ITransactionSource transaction = null)
        {
            if (this.TableNames == null)
            {
                var query = this.Database.QueryFactory.Create("SELECT TABLE_NAME FROM information_schema.tables");
                using (var reader = this.Database.ExecuteReader(query, transaction))
                {
                    this.TableNames = reader.Select(element => element.Get<string>("TABLE_NAME")).ToArray();
                }
            }
            return this.TableNames;
        }

        public IEnumerable<string> GetColumnNames(string tableName, ITransactionSource transaction = null)
        {
            var columnNames = default(string[]);
            if (!this.ColumnNames.TryGetValue(tableName, out columnNames))
            {
                var query = this.Database.QueryFactory.Create(string.Format("SELECT COLUMN_NAME FROM information_schema.columns WHERE TABLE_NAME = '{0}'", tableName));
                using (var reader = this.Database.ExecuteReader(query, transaction))
                {
                    columnNames = reader.Select(element => element.Get<string>("COLUMN_NAME")).ToArray();
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
