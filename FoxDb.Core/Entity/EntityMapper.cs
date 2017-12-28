using System;
using System.Collections.Generic;
using FoxDb.Interfaces;
using System.Linq;

namespace FoxDb
{
    public class EntityMapper : IEntityMapper
    {
        private EntityMapper()
        {
            this.Members = new DynamicMethod(this.GetType());
        }

        public EntityMapper(IDatabase database, Type tableType, bool includeRelations) : this()
        {
            this.Database = database;
            this.TableType = tableType;
            this.IncludeRelations = includeRelations;
        }

        protected DynamicMethod Members { get; private set; }

        public IDatabase Database { get; private set; }

        public Type TableType { get; private set; }

        public bool IncludeRelations { get; private set; }

        public IEnumerable<IRelationConfig> Relations
        {
            get
            {
                if (!this.IncludeRelations)
                {
                    yield break;
                }
                var queue = new Queue<ITableConfig>();
                queue.Enqueue(this.Database.Config.Table(this.TableType));
                while (queue.Count > 0)
                {
                    var table = queue.Dequeue();
                    foreach (var relation in table.Relations)
                    {
                        if (!relation.Behaviour.HasFlag(RelationBehaviour.EagerFetch))
                        {
                            continue;
                        }
                        queue.Enqueue(this.Database.Config.Table(relation.RelationType));
                        yield return relation;
                    }
                }
            }
        }

        public IEnumerable<ITableConfig> Tables
        {
            get
            {
                yield return this.Database.Config.Table(this.TableType);
                foreach (var relation in this.Relations)
                {
                    yield return this.Database.Config.Table(relation.RelationType);
                }
            }
        }

        public IEnumerable<IEntityColumnMap> GetColumns(ITableConfig table)
        {
            foreach (var column in table.Columns)
            {
                yield return this.GetColumn(column);
            }
        }

        public IEntityColumnMap GetColumn(IColumnConfig column)
        {
            return new EntityColumnMap(column, this.IncludeRelations);
        }

        public IDatabaseQuery Select
        {
            get
            {
                var compose = this.Database.QueryFactory.Compose();
                compose.Select();
                var first = true;
                foreach (var table in this.Tables)
                {
                    foreach (var column in this.GetColumns(table))
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
                        if (this.IncludeRelations)
                        {
                            compose.As();
                            compose.Identifier(column.Identifier);
                        }
                    }
                }
                compose.From();
                compose.Table(this.Database.Config.Table(this.TableType));
                if (this.IncludeRelations)
                {
                    foreach (var relation in this.Relations)
                    {
                        this.Members.Invoke(this, "OnSelectRelation", new[] { relation.Parent.TableType, relation.RelationType }, compose, relation);
                    }
                }
                compose.OrderBy();
                compose.Column(this.Database.Config.Table(this.TableType).PrimaryKey);
                if (this.IncludeRelations)
                {
                    foreach (var relation in this.Relations)
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
