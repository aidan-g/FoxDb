using FoxDb.Interfaces;
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
            this.Mapper = new EntityMapper(this.Table);
            this.Initializer = new EntityInitializer(this.Table, this.Mapper);
            this.Populator = new EntityPopulator(this.Table, this.Mapper);
            this.Factory = new EntityFactory(this.Table, this.Initializer, this.Populator);
            this.Composer = new EntityRelationQueryComposer(this.Database, this.Mapper);
            this.Transaction = transaction;
            this.Reset();
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

        public IEntityInitializer Initializer { get; private set; }

        public IEntityPopulator Populator { get; private set; }

        public IEntityFactory Factory { get; private set; }

        public IEntityRelationQueryComposer Composer { get; private set; }

        public IQueryGraphBuilder Fetch { get; set; }

        public IEnumerable<IQueryGraphBuilder> Add { get; set; }

        public IQueryGraphBuilder Update { get; set; }

        public IQueryGraphBuilder Delete { get; set; }

        public DatabaseParameterHandler Parameters { get; set; }

        public IDbTransaction Transaction { get; private set; }

        public void Reset()
        {
            this.Fetch = this.Composer.Query;
            this.Add = this.Database.QueryFactory.Add(this.Table);
            this.Update = this.Database.QueryFactory.Update(this.Table);
            this.Delete = this.Database.QueryFactory.Delete(this.Table);
        }
    }
}
