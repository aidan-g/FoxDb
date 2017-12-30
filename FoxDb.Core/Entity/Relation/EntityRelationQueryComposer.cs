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

        public IDatabaseQuery Select
        {
            get
            {
                var builder = this.Database.QueryFactory.Build();
                foreach (var table in this.Mapper.Tables)
                {
                    foreach (var column in this.Mapper.GetColumns(table))
                    {
                        var expression = builder.Select.AddColumn(column.Column);
                        if (this.Mapper.IncludeRelations)
                        {
                            expression.Alias = column.Identifier;
                        }
                    }
                }
                builder.From.AddTable(this.Mapper.Table);
                if (this.Mapper.IncludeRelations)
                {
                    foreach (var relation in this.Mapper.Relations)
                    {
                        builder.From.AddRelation(relation);
                    }
                }
                builder.OrderBy.AddColumn(this.Mapper.Table.PrimaryKey);
                if (this.Mapper.IncludeRelations)
                {
                    foreach (var relation in this.Mapper.Relations)
                    {
                        builder.OrderBy.AddColumn(relation.Table.PrimaryKey);
                    }
                }
                return this.Database.QueryFactory.Create(builder.Build());
            }
        }
    }
}
