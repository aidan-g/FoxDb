using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace FoxDb
{
    public class DatabaseQuerySource<T> : IDatabaseQuerySource
    {
        public DatabaseQuerySource(IDatabase database, IDbTransaction transaction = null) : this(database, false, transaction)
        {

        }

        public DatabaseQuerySource(IDatabase database, bool includeRelations = false, IDbTransaction transaction = null)
        {
            this.Database = database;
            this.Mapper = new EntityMapper(this.Database, this.Database.Config.Table<T>(), includeRelations);
            this.Composer = new EntityRelationQueryComposer(this.Database, this.Mapper);
            this.Select = this.Composer.Select;
            this.Find = this.Composer.Find;
            this.Insert = database.QueryFactory.Insert<T>();
            this.Update = database.QueryFactory.Update<T>();
            this.Delete = database.QueryFactory.Delete<T>();
            this.Transaction = transaction;
        }

        public IDatabase Database { get; private set; }

        public IEntityMapper Mapper { get; private set; }

        public IEntityRelationQueryComposer Composer { get; private set; }

        public IQueryGraphBuilder Select { get; set; }

        public IQueryGraphBuilder Find { get; set; }

        public IEnumerable<IQueryGraphBuilder> Insert { get; set; }

        public IQueryGraphBuilder Update { get; set; }

        public IQueryGraphBuilder Delete { get; set; }

        public DatabaseParameterHandler Parameters { get; set; }

        public IDbTransaction Transaction { get; set; }
    }
}
