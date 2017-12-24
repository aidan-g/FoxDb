using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SQLiteSchema : IDatabaseSchema
    {
        private SQLiteSchema()
        {
            this.ColumnNames = new Dictionary<Type, IEnumerable<string>>();
        }

        public SQLiteSchema(IDatabase database) : this()
        {
            this.Database = database;
        }

        public IDatabase Database { get; private set; }

        protected IDictionary<Type, IEnumerable<string>> ColumnNames { get; set; }

        public IEnumerable<string> GetColumnNames<T>() where T : IPersistable
        {
            var key = typeof(T);
            if (!this.ColumnNames.ContainsKey(key))
            {
                var table = this.Database.Config.Table<T>();
                var query = new DatabaseQuery(string.Format("PRAGMA table_info('{0}')", table.TableName));
                using (var reader = this.Database.ExecuteReader(query))
                {
                    this.ColumnNames[key] = reader.Select(element => element.Get<string>("name")).ToArray();
                }
                if (!this.ColumnNames.Any())
                {
                    throw new InvalidOperationException(string.Format("Table \"{0}\" does not exist.", table.TableName));
                }
            }
            return this.ColumnNames[key];
        }
    }
}
