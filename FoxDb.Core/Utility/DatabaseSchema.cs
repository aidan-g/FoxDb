using FoxDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FoxDb
{
    public abstract class DatabaseSchema : IDatabaseSchema
    {
        private DatabaseSchema()
        {
            this.SupportedTypes = new[]
            {
                //Integer.
                DbType.Byte,
                DbType.Int16, 
                DbType.Int32, 
                DbType.Int64, 
                DbType.UInt16,
                DbType.UInt32,
                DbType.UInt64,
                //Floating.
                DbType.Single, 
                DbType.Double, 
                DbType.Decimal,
                DbType.VarNumeric, 
                //Boolean
                DbType.Boolean,
                //String.
                DbType.AnsiString, 
                DbType.AnsiStringFixedLength, 
                DbType.String, 
                DbType.StringFixedLength, 
                //Other
                DbType.Guid, 
                DbType.Binary
            };
            this.Reset();
        }

        public DatabaseSchema(IDatabase database)
            : this()
        {
            this.Database = database;
        }

        public IDatabase Database { get; private set; }

        protected IEnumerable<string> TableNames { get; set; }

        protected ConcurrentDictionary<string, string[]> ColumnNames { get; set; }

        public IEnumerable<DbType> SupportedTypes { get; private set; }

        public bool TableExists(string tableName, ITransactionSource transaction = null)
        {
            return this.GetTableNames(transaction).Contains(tableName, StringComparer.OrdinalIgnoreCase);
        }

        public abstract IEnumerable<string> GetTableNames(ITransactionSource transaction = null);

        public bool ColumnExists(string tableName, string columnName, ITransactionSource transaction = null)
        {
            return this.GetColumnNames(tableName, transaction).Contains(columnName, StringComparer.OrdinalIgnoreCase);
        }

        public abstract IEnumerable<string> GetColumnNames(string tableName, ITransactionSource transaction = null);

        public void Reset()
        {
            this.TableNames = null;
            this.ColumnNames = new ConcurrentDictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
