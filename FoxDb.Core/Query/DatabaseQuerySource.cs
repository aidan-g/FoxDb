using FoxDb.Interfaces;

namespace FoxDb
{
    public class DatabaseQuerySource : IDatabaseQuerySource
    {
        public DatabaseQuerySource(IDatabase database, IDatabaseQueryComposer composer, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            this.Database = database;
            this.Composer = composer;
            this.OriginalParameters = parameters;
            this.Transaction = transaction;
            this.Reset();
        }

        public DatabaseQuerySource(IDatabase database, IDatabaseQueryComposer composer, DatabaseParameterHandler parameters, IQueryGraphBuilder fetch, IQueryGraphBuilder add, IQueryGraphBuilder update, IQueryGraphBuilder delete, ITransactionSource transaction = null) : this(database, composer, parameters, transaction)
        {
            this.Fetch = fetch;
            this.Add = add;
            this.Update = update;
            this.Delete = delete;
        }

        public IDatabase Database { get; private set; }

        public IDatabaseQueryComposer Composer { get; private set; }

        public DatabaseParameterHandler Parameters { get; set; }

        public DatabaseParameterHandler OriginalParameters { get; private set; }

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
            this.Parameters = this.OriginalParameters;
        }

        public IDatabaseQuerySource Clone()
        {
            return new DatabaseQuerySource(this.Database, this.Composer, this.Parameters, this.Transaction);
        }
    }
}
