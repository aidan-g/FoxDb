using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SQLiteSchema : DatabaseSchema
    {
        public SQLiteSchema(IDatabase database)
            : base(database)
        {

        }

        public override IEnumerable<string> GetTableNames(ITransactionSource transaction = null)
        {
            if (this.TableNames == null)
            {
                var query = this.Database.QueryFactory.Create("SELECT name FROM sqlite_master WHERE type = 'table'");
                using (var reader = this.Database.ExecuteReader(query, transaction))
                {
                    this.TableNames = reader.Select(element => element.Get<string>("name")).ToArray();
                }
            }
            return this.TableNames;
        }

        public override IEnumerable<string> GetColumnNames(string tableName, ITransactionSource transaction = null)
        {
            var columnNames = default(string[]);
            if (!this.ColumnNames.TryGetValue(tableName, out columnNames))
            {
                var query = this.Database.QueryFactory.Create(string.Format("PRAGMA table_info('{0}')", tableName));
                using (var reader = this.Database.ExecuteReader(query, transaction))
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
