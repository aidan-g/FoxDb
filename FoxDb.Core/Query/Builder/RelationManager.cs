using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class RelationManager : IRelationManager
    {
        public RelationManager(IQueryGraphBuilder graph)
        {
            this.Calculator = new EntityRelationCalculator(GetTables(graph));
        }

        public IEntityRelationCalculator Calculator { get; set; }

        public IEnumerable<IRelationConfig> Relations
        {
            get
            {
                return this.Calculator.Relations;
            }
        }

        public IEnumerable<IEntityRelation> CalculatedRelations
        {
            get
            {
                return this.Calculator.CalculatedRelations;
            }
        }

        public void AddRelation(IRelationConfig relation)
        {
            this.Calculator.AddRelation(relation);
        }

        public bool HasExternalRelations
        {
            get
            {
                return this.CalculatedRelations.Any(relation => relation.IsExternal);
            }
        }

        private static IEnumerable<ITableConfig> GetTables(IQueryGraphBuilder graph)
        {
            return graph.Fragments
                .OfType<ISourceBuilder>()
                .SelectMany(
                    source => source.Tables.Select(table => table.Table)
                );
        }
    }
}
