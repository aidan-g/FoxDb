﻿using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace FoxDb
{
    public class DatabaseQuerySource : IDatabaseQuerySource
    {
        public DatabaseQuerySource(IDatabase database, ITableConfig table, IDbTransaction transaction = null)
        {
            this.Database = database;
            this.Table = table;
            this.Mapper = new EntityMapper(table);
            this.Composer = new EntityRelationQueryComposer(this.Database, this.Mapper);
            this.Fetch = this.Composer.Query;
            this.Add = database.QueryFactory.Add(table);
            this.Update = database.QueryFactory.Update(table);
            this.Delete = database.QueryFactory.Delete(table);
            this.Transaction = transaction;
        }

        public IDatabase Database { get; private set; }

        public ITableConfig Table { get; private set; }

        public bool CanRead
        {
            get
            {
                return this.Fetch != null;
            }
        }

        public bool CanWrite
        {
            get
            {
                return this.Add != null && this.Update != null && this.Delete != null;
            }
        }

        public IEntityMapper Mapper { get; private set; }

        public IEntityRelationQueryComposer Composer { get; private set; }

        public IQueryGraphBuilder Fetch { get; set; }

        public IEnumerable<IQueryGraphBuilder> Add { get; set; }

        public IQueryGraphBuilder Update { get; set; }

        public IQueryGraphBuilder Delete { get; set; }

        public DatabaseParameterHandler Parameters { get; set; }

        public IDbTransaction Transaction { get; private set; }
    }
}
