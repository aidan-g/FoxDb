using FoxDb.Interfaces;

namespace FoxDb
{
    public class EntityRelationQueryComposer : IEntityRelationQueryComposer
    {
        private EntityRelationQueryComposer()
        {
            this.Members = new DynamicMethod(this.GetType());
        }

        public EntityRelationQueryComposer(IDatabase database, IEntityMapper mapper) : this()
        {
            this.Database = database;
            this.Mapper = mapper;
        }

        protected DynamicMethod Members { get; private set; }

        public IDatabase Database { get; private set; }

        public IEntityMapper Mapper { get; private set; }

        public IQueryGraphBuilder Select
        {
            get
            {
                var builder = this.Database.QueryFactory.Build();
                foreach (var table in this.Mapper.Tables)
                {
                    builder.Select.AddColumns(table.Columns);
                }
                builder.From.AddTable(this.Mapper.Table);
                foreach (var relation in this.Mapper.Relations)
                {
                    builder.From.AddRelation(relation);
                }
                builder.OrderBy.AddColumn(this.Mapper.Table.PrimaryKey);
                foreach (var relation in this.Mapper.Relations)
                {
                    builder.OrderBy.AddColumn(relation.RightTable.PrimaryKey);
                }
                return builder;
            }
        }

        public IQueryGraphBuilder Find
        {
            get
            {
                var query = this.Select;
                query.Where.AddColumns(this.Mapper.Table.PrimaryKeys);
                return query;
            }
        }
    }
}
