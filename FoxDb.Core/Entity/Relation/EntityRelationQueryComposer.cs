using FoxDb.Interfaces;

namespace FoxDb
{
    public class EntityRelationQueryComposer : DatabaseQueryComposer
    {
        public EntityRelationQueryComposer(IDatabase database, ITableConfig table) : base(database, table)
        {
            this.Mapper = new EntityMapper(this.Table);
        }

        public IEntityMapper Mapper { get; private set; }

        public override IQueryGraphBuilder Fetch
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
                    builder.RelationManager.AddRelation(relation);
                }
                if (this.Mapper.Table.PrimaryKey != null)
                {
                    builder.Sort.AddColumn(this.Mapper.Table.PrimaryKey);
                }
                foreach (var relation in this.Mapper.Relations)
                {
                    builder.Sort.AddColumn(relation.RightTable.PrimaryKey);
                }
                return builder;
            }
        }
    }
}
