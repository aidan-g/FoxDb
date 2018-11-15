using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SqlDropWriter : SqlQueryWriter
    {
        public SqlDropWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Drop;
            }
        }

        protected override T OnWrite<T>(T fragment)
        {
            if (fragment is IDropBuilder)
            {
                var expression = fragment as IDropBuilder;
                var tables = expression.Expressions.OfType<ITableBuilder>();
                var relations = expression.Expressions.OfType<IRelationBuilder>();
                var indexes = expression.Expressions.OfType<IIndexBuilder>();
                var batches = new List<Action>();
                foreach (var table in tables)
                {
                    batches.Add(() => this.VisitTable(expression, table));
                }
                foreach (var relation in relations)
                {
                    batches.Add(() => this.VisitRelation(expression, relation));
                }
                foreach (var index in indexes)
                {
                    batches.Add(() => this.VisitIndex(expression, index));
                }
                this.VisitBatches(batches);
                return fragment;
            }
            throw new NotImplementedException();
        }

        protected virtual void VisitTable(IDropBuilder expression, ITableBuilder table)
        {
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.DROP);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.TABLE);
            this.Visit(table);
        }

        protected virtual void VisitRelation(IDropBuilder expression, IRelationBuilder relation)
        {
            switch (relation.Relation.Flags.GetMultiplicity())
            {
                case RelationFlags.OneToOne:
                case RelationFlags.OneToMany:
                    this.VisitRelation(expression, relation, relation.Relation.LeftTable, relation.Relation.RightTable);
                    break;
                case RelationFlags.ManyToMany:
                    this.VisitRelation(expression, relation, relation.Relation.LeftTable, relation.Relation.MappingTable);
                    this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.BATCH);
                    this.VisitRelation(expression, relation, relation.Relation.RightTable, relation.Relation.MappingTable);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual void VisitRelation(IDropBuilder expression, IRelationBuilder relation, ITableConfig leftTable, ITableConfig rightTable)
        {
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.ALTER);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.TABLE);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.Identifier(rightTable.TableName));
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.DROP);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CONSTRAINT);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.Identifier(Conventions.RelationName(leftTable, rightTable)));
        }

        protected virtual void VisitIndex(IDropBuilder expression, IIndexBuilder index)
        {
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.DROP);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.INDEX);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.Identifier(Conventions.IndexName(index.Index)));
        }

        public override IFragmentBuilder Clone()
        {
            throw new NotImplementedException();
        }
    }
}
