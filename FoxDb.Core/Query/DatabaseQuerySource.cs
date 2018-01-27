﻿using FoxDb.Interfaces;

namespace FoxDb
{
    public class DatabaseQuerySource : IDatabaseQuerySource
    {
        public DatabaseQuerySource(IDatabase database, IDatabaseQueryComposer composer, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            this.Database = database;
            this.Composer = composer;
            this.Parameters = parameters;
            this.Transaction = transaction;
            this.Reset();
        }

        public IDatabase Database { get; private set; }

        public IDatabaseQueryComposer Composer { get; private set; }

        public DatabaseParameterHandler Parameters { get; set; }

        public ITransactionSource Transaction { get; private set; }

        public IQueryGraphBuilder Fetch { get; set; }

        public IQueryGraphBuilder Add { get; set; }

        public IQueryGraphBuilder Update { get; set; }

        public IQueryGraphBuilder Delete { get; set; }

        public void Reset()
        {
            this.Fetch = this.Composer.Fetch;
            this.Add = this.Composer.Add;
            this.Update = this.Composer.Update;
            this.Delete = this.Composer.Delete;
        }

        public IDatabaseQuerySource Clone()
        {
            return new DatabaseQuerySource(this.Database, this.Composer, this.Parameters, this.Transaction);
        }
    }
}
