using FoxDb.Interfaces;

namespace FoxDb
{
    public class DatabaseQuerySource : IDatabaseQuerySource
    {
        public DatabaseQuerySource(IDatabase database, ITableConfig table, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            this.Database = database;
            this.Table = table;
            this.Parameters = parameters;
            this.Transaction = transaction;
            this.Mapper = new EntityMapper(this.Table);
            this.Composer = new EntityRelationQueryComposer(this.Database, this.Mapper);
            this.Reset();
        }

        public IDatabase Database { get; private set; }

        public ITableConfig Table { get; private set; }

        public DatabaseParameterHandler Parameters { get; set; }

        public ITransactionSource Transaction { get; private set; }

        public IEntityMapper Mapper { get; private set; }

        public IEntityInitializer Initializer { get; private set; }

        public IEntityPopulator Populator { get; private set; }

        public IEntityFactory Factory { get; private set; }

        public IEntityRelationQueryComposer Composer { get; private set; }

        public IQueryGraphBuilder Fetch { get; set; }

        public IQueryGraphBuilder Add { get; set; }

        public IQueryGraphBuilder Update { get; set; }

        public IQueryGraphBuilder Delete { get; set; }

        public void Reset()
        {
            if (this.Composer != null)
            {
                this.Fetch = this.Composer.Query;
            }
            this.Add = this.Database.QueryFactory.Add(this.Table);
            this.Update = this.Database.QueryFactory.Update(this.Table);
            this.Delete = this.Database.QueryFactory.Delete(this.Table);
        }
    }
}
