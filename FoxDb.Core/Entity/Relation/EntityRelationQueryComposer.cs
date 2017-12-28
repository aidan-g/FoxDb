using FoxDb.Interfaces;
using System;

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
                var compose = this.Database.QueryFactory.Compose();
                compose.Select();
                var first = true;
                foreach (var table in this.Mapper.Tables)
                {
                    foreach (var column in this.Mapper.GetColumns(table))
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            compose.ListDelimiter();
                        }
                        compose.Column(column.Column);
                        if (this.Mapper.IncludeRelations)
                        {
                            compose.As();
                            compose.Identifier(column.Identifier);
                        }
                    }
                }
                compose.From();
                compose.Table(this.Mapper.Table);
                if (this.Mapper.IncludeRelations)
                {
                    foreach (var relation in this.Mapper.Relations)
                    {
                        this.Members.Invoke(this, "OnSelectRelation", new[] { relation.Parent.TableType, relation.RelationType }, compose, relation);
                    }
                }
                compose.OrderBy();
                compose.Column(this.Mapper.Table.PrimaryKey);
                if (this.Mapper.IncludeRelations)
                {
                    foreach (var relation in this.Mapper.Relations)
                    {
                        compose.ListDelimiter();
                        compose.Column(relation.Table.PrimaryKey);
                    }
                }
                return compose.Query;
            }
        }

        protected virtual void OnSelectRelation<T, TRelation>(IDatabaseQueryComposer compose, IRelationConfig<T, TRelation> relation)
        {
            compose.Join();
            compose.Table(relation.Table);
            compose.On();
            compose.Column(relation.Parent.PrimaryKey);
            compose.Equal();
            compose.Column(relation.Table.ForeignKey);
        }

        protected virtual void OnSelectRelation<T, TRelation>(IDatabaseQueryComposer compose, ICollectionRelationConfig<T, TRelation> relation)
        {
            switch (relation.Multiplicity)
            {
                case RelationMultiplicity.OneToMany:
                    compose.Join();
                    compose.Table(relation.Table);
                    compose.On();
                    compose.Column(relation.Parent.PrimaryKey);
                    compose.Equal();
                    compose.Column(relation.Table.ForeignKey);
                    break;
                case RelationMultiplicity.ManyToMany:
                    var table = this.Database.Config.Table(relation.Parent.TableType, relation.RelationType);
                    compose.Join();
                    compose.Table(table);
                    compose.On();
                    compose.Column(table.LeftForeignKey);
                    compose.Equal();
                    compose.Column(table.LeftTable.PrimaryKey);
                    compose.Join();
                    compose.Table(table.RightTable);
                    compose.On();
                    compose.Column(table.RightForeignKey);
                    compose.Equal();
                    compose.Column(table.RightTable.PrimaryKey);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
