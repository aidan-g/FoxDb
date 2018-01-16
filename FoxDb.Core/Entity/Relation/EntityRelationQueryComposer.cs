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

        public IQueryGraphBuilder Query
        {
            get
            {
                var builder = this.Database.QueryFactory.Build();
                foreach (var table in this.Mapper.Tables)
                {
                    builder.Output.AddColumns(table.Columns);
                }
                builder.Source.AddTable(this.Mapper.Table);
                foreach (var relation in this.Mapper.Relations)
                {
                    builder.Source.AddRelation(relation);
                }
                builder.Sort.AddColumn(this.Mapper.Table.PrimaryKey);
                foreach (var relation in this.Mapper.Relations)
                {
                    builder.Sort.AddColumn(relation.RightTable.PrimaryKey);
                }
                return builder;
            }
        }
    }
}
