using FoxDb.Interfaces;

namespace FoxDb
{
    public class DatabaseQueryComposer : IDatabaseQueryComposer
    {
        public DatabaseQueryComposer(IDatabase database, ITableConfig table)
        {
            this.Database = database;
            this.Table = table;
        }

        public IDatabase Database { get; private set; }

        public ITableConfig Table { get; private set; }

        public virtual IQueryGraphBuilder Fetch
        {
            get
            {
                return this.Database.QueryFactory.Fetch(this.Table);
            }
        }

        public virtual IQueryGraphBuilder Add
        {
            get
            {
                return this.Database.QueryFactory.Add(this.Table);
            }
        }

        public virtual IQueryGraphBuilder Update
        {
            get
            {
                return this.Database.QueryFactory.Update(this.Table);
            }
        }

        public virtual IQueryGraphBuilder Delete
        {
            get
            {
                return this.Database.QueryFactory.Delete(this.Table);
            }
        }
    }

    public class DatabaseQueryComposer<T> : DatabaseQueryComposer
    {
#pragma warning disable 612, 618
        public DatabaseQueryComposer(IDatabase database) : base(database, database.Config.Table<T>())
#pragma warning restore 612, 618
        {

        }
    }
}
