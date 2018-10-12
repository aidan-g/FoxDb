using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class CreateBuilder : FragmentBuilder, ICreateBuilder
    {
        public CreateBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
            : base(parent, graph)
        {
            this.Expressions = new List<IFragmentBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Create;
            }
        }

        public ICollection<IFragmentBuilder> Expressions { get; private set; }

        public ITableBuilder GetTable(ITableConfig table)
        {
            return this.GetExpression<ITableBuilder>(builder => TableComparer.TableConfig.Equals(builder.Table, table));
        }

        public ITableBuilder AddTable(ITableConfig table)
        {
            var builder = this.CreateTable(table);
            this.Expressions.Add(builder);
            return builder;
        }

        public ICreateBuilder AddTables(IEnumerable<ITableConfig> tables)
        {
            foreach (var table in tables)
            {
                this.AddTable(table);
            }
            return this;
        }

        public IColumnBuilder GetColumn(IColumnConfig column)
        {
            return this.GetExpression<IColumnBuilder>(builder => ColumnComparer.ColumnConfig.Equals(builder.Column, column));
        }

        public IColumnBuilder AddColumn(IColumnConfig column)
        {
            var builder = this.CreateColumn(column);
            this.Expressions.Add(builder);
            return builder;
        }

        public ICreateBuilder AddColumns(IEnumerable<IColumnConfig> columns)
        {
            foreach (var column in columns)
            {
                this.AddColumn(column);
            }
            return this;
        }

        public IRelationBuilder GetRelation(IRelationConfig relation)
        {
            return this.GetExpression<IRelationBuilder>(builder => builder.Relation == relation);
        }

        public IRelationBuilder AddRelation(IRelationConfig relation)
        {
            var builder = this.CreateRelation(relation);
            this.Expressions.Add(builder);
            return builder;
        }

        public ICreateBuilder AddRelations(IEnumerable<IRelationConfig> relations)
        {
            foreach (var relation in relations)
            {
                this.AddRelation(relation);
            }
            return this;
        }

        public IIndexBuilder GetIndex(IIndexConfig index)
        {
            return this.GetExpression<IIndexBuilder>(builder => IndexComparer.IndexConfig.Equals(builder.Index, index));
        }

        public IIndexBuilder AddIndex(IIndexConfig index)
        {
            var builder = this.CreateIndex(index);
            this.Expressions.Add(builder);
            return builder;
        }

        public ICreateBuilder AddIndexes(IEnumerable<IIndexConfig> indexes)
        {
            foreach (var index in indexes)
            {
                this.AddIndex(index);
            }
            return this;
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<ICreateBuilder>().With(builder =>
            {
                foreach (var expression in this.Expressions)
                {
                    builder.Expressions.Add(expression.Clone());
                }
            });
        }
    }
}
