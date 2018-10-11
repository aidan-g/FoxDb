using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class EntityRelationCalculator : IEntityRelationCalculator
    {
        private EntityRelationCalculator()
        {
            this.Relations = new HashSet<IRelationConfig>();
        }

        public EntityRelationCalculator(ITableConfig table)
            : this(new[] { table })
        {

        }

        public EntityRelationCalculator(IEnumerable<ITableConfig> tables)
            : this()
        {
            this.Tables = tables;
        }

        public ICollection<IRelationConfig> Relations { get; private set; }

        public IEnumerable<ITableConfig> Tables { get; private set; }

        public IEnumerable<ITableConfig> InternalTables
        {
            get
            {
                return this.Tables.Where(table => this.IsExternalTable(table));
            }
        }

        public IEnumerable<ITableConfig> ExternalTables
        {
            get
            {
                return this.Tables.Where(table => !this.IsExternalTable(table));
            }
        }

        public bool IsExternalTable(ITableConfig table)
        {
            return !table.Flags.HasFlag(TableFlags.Extern);
        }

        public ITableConfig GetTable(ITableConfig table)
        {
            return this.Tables.FirstOrDefault(TableComparer.Equals(table)) ?? table;
        }

        public IEnumerable<ITableConfig> GetTables(IEnumerable<ITableConfig> tables)
        {
            return tables.Select(table => this.GetTable(table));
        }

        IEnumerable<IRelationConfig> IEntityRelationCalculator.Relations
        {
            get
            {
                return this.Relations;
            }
        }

        public void AddRelation(IRelationConfig relation)
        {
            this.Relations.Add(relation);
            this.CalculatedRelations = null;
        }

        public void AddRelations(IEnumerable<IRelationConfig> relations)
        {
            foreach (var relation in relations)
            {
                this.AddRelation(relation);
            }
        }

        private IEnumerable<IEntityRelation> _CalculatedRelations { get; set; }

        public IEnumerable<IEntityRelation> CalculatedRelations
        {
            get
            {
                if (this._CalculatedRelations == null)
                {
                    this._CalculatedRelations = this.CalculateRelations();
                }
                return this._CalculatedRelations;
            }
            private set
            {
                this._CalculatedRelations = value;
            }
        }

        protected virtual IEnumerable<IEntityRelation> CalculateRelations()
        {
            var observedTables = new HashSet<ITableConfig>(this.InternalTables);
            var relations = new List<EntityRelation>();
            foreach (var relation in this.GetRelations())
            {
                var referencedTables = this.GetTables(relation.Expression.GetTables())
                    .Except(observedTables)
                    .ToArray();
                if (referencedTables.Length > 1)
                {
                    var externalTable = referencedTables.FirstOrDefault(table => !this.IsExternalTable(table));
                    if (externalTable != null)
                    {
                        referencedTables = new[] { externalTable };
                    }
                    else
                    {
                        referencedTables = referencedTables.Except(this.Tables).ToArray();
                    }
                }
                if (referencedTables.Length == 1)
                {
                    var referencedTable = referencedTables[0];
                    relations.Add(new EntityRelation(relation.Relation, referencedTable, relation.Expression));
                    observedTables.Add(referencedTable);
                }
                else
                {
                    var success = false;
                    foreach (var existingRelation in Enumerable.Reverse(relations))
                    {
                        if (this.HasCommonParent(existingRelation.Expression, relation.Expression))
                        {
                            existingRelation.Expression = this.Combine(existingRelation.Expression, relation.Expression);
                            success = true;
                            break;
                        }
                    }
                    if (!success)
                    {
                        foreach (var existingRelation in Enumerable.Reverse(relations))
                        {
                            if (this.HasCommonTable(existingRelation.Expression, relation.Expression))
                            {
                                existingRelation.Expression = this.Combine(existingRelation.Expression, relation.Expression, QueryOperator.OrElse);
                                success = true;
                                break;
                            }
                        }
                        if (!success)
                        {
                            throw new NotImplementedException();
                        }
                    }
                }
            }
            return relations;
        }

        public IBinaryExpressionBuilder Extern(IEntityRelation relation)
        {
            var expression = (IBinaryExpressionBuilder)relation.Expression.Clone();
            if (!this.Extern(relation, expression))
            {
                throw new NotImplementedException();
            }
            return expression;
        }

        protected virtual bool Extern(IEntityRelation relation, IBinaryExpressionBuilder expression)
        {
            //Nothing to do.
            return true;
        }

        protected virtual bool HasCommonParent(IBinaryExpressionBuilder left, IBinaryExpressionBuilder right)
        {
            var parent = left.Parent as IBinaryExpressionBuilder ?? right.Parent as IBinaryExpressionBuilder;
            if (parent != null)
            {
                if (new[] { parent.Left, parent.Right }.Contains(new[] { left, right }))
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual bool HasCommonTable(IBinaryExpressionBuilder left, IBinaryExpressionBuilder right)
        {
            return left.GetTables().Intersect(right.GetTables()).Any();
        }

        protected virtual IBinaryExpressionBuilder Combine(IBinaryExpressionBuilder left, IBinaryExpressionBuilder right, QueryOperator @operator = QueryOperator.None)
        {
            if (object.ReferenceEquals(left, right))
            {
                return left ?? right;
            }
            if (this.HasCommonParent(left, right))
            {
                var expression = left.Parent ?? right.Parent;
                if (expression is IBinaryExpressionBuilder)
                {
                    return (IBinaryExpressionBuilder)expression;
                }
            }
            var parent = left.Parent as IBinaryExpressionBuilder ?? right.Parent as IBinaryExpressionBuilder;
            if (parent != null)
            {
                return parent.Fragment<IBinaryExpressionBuilder>().With(expression =>
                {
                    expression.Left = left;
                    expression.Operator = @operator == QueryOperator.None ? parent.Operator : parent.CreateOperator(@operator);
                    expression.Right = right;
                });
            }
            throw new NotImplementedException();
        }

        protected virtual IEnumerable<PendingEntityRelation> GetRelations()
        {
            if (!this.Relations.Any())
            {
                return Enumerable.Empty<PendingEntityRelation>();
            }
            var observedTables = new HashSet<ITableConfig>(this.Tables);
            var internalRelations = new List<PendingEntityRelation>();
            var externalRelations = new List<PendingEntityRelation>();
            foreach (var relation in this.Relations)
            {
                var remainingExpressions = relation.Expression
                    .GetLeaves()
                    .ToList();
                while (remainingExpressions.Count > 0)
                {
                    var limit = 0;
                    var success = false;
                    while (!success)
                    {
                        foreach (var expression in remainingExpressions.ToList())
                        {
                            var referencedTables = this.GetTables(expression.GetTables()).ToArray();
                            var extendedTable = referencedTables.FirstOrDefault(table => !this.IsExternalTable(table));
                            if (extendedTable != null)
                            {
                                externalRelations.Add(new PendingEntityRelation(relation, expression));
                                remainingExpressions.Remove(expression);
                                observedTables.Add(extendedTable);
                                success = true;
                                break;
                            }
                            var dependentTables = referencedTables
                                .Except(observedTables.Concat(relation.MappingTable ?? relation.RightTable))
                                .ToArray();
                            if (dependentTables.Length <= limit)
                            {
                                internalRelations.Add(new PendingEntityRelation(relation, expression));
                                remainingExpressions.Remove(expression);
                                observedTables.AddRange(dependentTables);
                                success = true;
                                break;
                            }
                        }
                        limit++;
                    }
                }
            }
            return internalRelations.Concat(externalRelations);
        }

        public IEntityRelationCalculator Clone()
        {
            return new EntityRelationCalculator(this.Tables).With(calculator =>
            {
                calculator.Relations.AddRange(this.Relations);
            });
        }

        protected class PendingEntityRelation
        {
            public PendingEntityRelation(IRelationConfig relation, IBinaryExpressionBuilder expression)
            {
                this.Relation = relation;
                this.Expression = expression;
            }

            public IRelationConfig Relation { get; private set; }

            public IBinaryExpressionBuilder Expression { get; private set; }
        }

        private class EntityRelation : IEntityRelation
        {
            public EntityRelation(IRelationConfig relation, ITableConfig table, IBinaryExpressionBuilder expression)
            {
                this.Relation = relation;
                this.Table = table;
                this.Expression = expression;
            }

            public IRelationConfig Relation { get; private set; }

            public ITableConfig Table { get; private set; }

            public IBinaryExpressionBuilder Expression { get; set; }

            public bool IsExternal
            {
                get
                {
                    return this.Flags.HasFlag(EntityRelationFlags.Extern);
                }
            }

            public EntityRelationFlags Flags
            {
                get
                {
                    var flags = EntityRelationFlags.None;
                    if (this.Table.Flags.HasFlag(TableFlags.Extern))
                    {
                        flags |= EntityRelationFlags.Extern;
                    }
                    return flags;
                }
            }
        }
    }
}
