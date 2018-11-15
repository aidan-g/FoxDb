using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public abstract class SqlSchemaFactory : IDatabaseSchemaFactory
    {
        public SqlSchemaFactory(IDatabase database)
        {
            this.Database = database;
        }

        public IDatabase Database { get; private set; }

        public abstract IDatabaseQueryDialect Dialect { get; }

        public ISchemaGraphBuilder Build()
        {
            return new SchemaGraphBuilder(this.Database);
        }

        public ISchemaGraphBuilder Combine(IEnumerable<ISchemaGraphBuilder> graphs)
        {
            return new AggregateSchemaGraphBuilder(this.Database, graphs);
        }

        public ISchemaGraphBuilder Add(ITableConfig table, SchemaFlags flags)
        {
            var tables = default(HashSet<ITableConfig>);
            var indexes = default(HashSet<IIndexConfig>);
            var relations = default(HashSet<IRelationConfig>);
            this.GetSchema(table, out tables, out indexes, out relations, flags);
            return this.OnAdd(tables, indexes, relations, flags);
        }

        protected virtual ISchemaGraphBuilder OnAdd(IEnumerable<ITableConfig> tables, IEnumerable<IIndexConfig> indexes, IEnumerable<IRelationConfig> relations, SchemaFlags flags)
        {
            var queries = new List<ISchemaGraphBuilder>();
            if (flags.HasFlag(SchemaFlags.Table))
            {
                queries.Add(this.OnAddTables(tables));
            }
            if (flags.HasFlag(SchemaFlags.Index))
            {
                queries.Add(this.OnAddIndexes(indexes));
            }
            if (flags.HasFlag(SchemaFlags.Relation))
            {
                queries.Add(this.OnAddRelations(relations));
            }
            return new AggregateSchemaGraphBuilder(
                this.Database,
                queries
            );
        }

        protected virtual ISchemaGraphBuilder OnAddTables(IEnumerable<ITableConfig> tables)
        {
            var builder = this.Build();
            builder.Create.AddTables(tables);
            foreach (var table in tables)
            {
                builder.Create.AddColumns(table.Columns);
            }
            return builder;
        }

        protected virtual ISchemaGraphBuilder OnAddIndexes(IEnumerable<IIndexConfig> indexes)
        {
            var builder = this.Build();
            builder.Create.AddIndexes(indexes);
            return builder;
        }

        protected virtual ISchemaGraphBuilder OnAddRelations(IEnumerable<IRelationConfig> relations)
        {
            var builder = this.Build();
            builder.Create.AddRelations(relations);
            return builder;
        }

        public ISchemaGraphBuilder Update(ITableConfig leftTable, ITableConfig rightTable, SchemaFlags flags)
        {
            var builder = this.Build();
            builder.Alter.SetLeftTable(leftTable);
            builder.Alter.SetRightTable(rightTable);
            return builder;
        }

        public ISchemaGraphBuilder Delete(ITableConfig table, SchemaFlags flags)
        {
            var tables = default(HashSet<ITableConfig>);
            var indexes = default(HashSet<IIndexConfig>);
            var relations = default(HashSet<IRelationConfig>);
            this.GetSchema(table, out tables, out indexes, out relations, flags);
            return this.OnDelete(tables, indexes, relations, flags);
        }

        protected virtual ISchemaGraphBuilder OnDelete(IEnumerable<ITableConfig> tables, IEnumerable<IIndexConfig> indexes, IEnumerable<IRelationConfig> relations, SchemaFlags flags)
        {
            var queries = new List<ISchemaGraphBuilder>();
            if (flags.HasFlag(SchemaFlags.Relation))
            {
                queries.Add(this.OnDeleteRelations(relations));
            }
            if (flags.HasFlag(SchemaFlags.Index))
            {
                queries.Add(this.OnDeleteIndexes(indexes));
            }
            if (flags.HasFlag(SchemaFlags.Table))
            {
                queries.Add(this.OnDeleteTables(tables));
            }
            return new AggregateSchemaGraphBuilder(
                this.Database,
                queries
            );
        }

        protected virtual ISchemaGraphBuilder OnDeleteTables(IEnumerable<ITableConfig> tables)
        {
            var builder = this.Build();
            builder.Drop.AddTables(tables);
            return builder;
        }

        protected virtual ISchemaGraphBuilder OnDeleteIndexes(IEnumerable<IIndexConfig> indexes)
        {
            var builder = this.Build();
            builder.Drop.AddIndexes(indexes);
            return builder;
        }

        protected virtual ISchemaGraphBuilder OnDeleteRelations(IEnumerable<IRelationConfig> relations)
        {
            var builder = this.Build();
            builder.Drop.AddRelations(relations);
            return builder;
        }

        protected virtual void GetSchema(ITableConfig table, out HashSet<ITableConfig> tables, out HashSet<IIndexConfig> indexes, out HashSet<IRelationConfig> relations, SchemaFlags flags)
        {
            tables = new HashSet<ITableConfig>();
            indexes = new HashSet<IIndexConfig>();
            relations = new HashSet<IRelationConfig>();
            if (flags.HasFlag(SchemaFlags.Recursive))
            {
                var queue = new Queue<ITableConfig>();
                queue.Enqueue(table);
                while (queue.Count > 0)
                {
                    table = queue.Dequeue();
                    if (!tables.Add(table))
                    {
                        continue;
                    }
                    foreach (var index in table.Indexes)
                    {
                        indexes.Add(index);
                    }
                    foreach (var relation in table.Relations)
                    {
                        relations.Add(relation);
                        if (relation.MappingTable != null && !relation.MappingTable.Flags.HasFlag(TableFlags.Shared))
                        {
                            queue.Enqueue(relation.MappingTable);
                        }
                        if (relation.RightTable != null && !relation.RightTable.Flags.HasFlag(TableFlags.Shared))
                        {
                            queue.Enqueue(relation.RightTable);
                        }
                    }
                }
            }
            else
            {
                tables.Add(table);
                indexes.AddRange(table.Indexes);
                relations.AddRange(table.Relations);
            }
        }
    }
}
