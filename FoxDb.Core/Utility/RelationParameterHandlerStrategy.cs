using FoxDb.Interfaces;

namespace FoxDb
{
    public class RelationParameterHandlerStrategy<T, TRelation> : IParameterHandlerStrategy
        where T : IPersistable
        where TRelation : IPersistable
    {
        public RelationParameterHandlerStrategy(T item, IRelationConfig relation)
        {
            this.Item = item;
            this.Relation = relation;
        }

        public T Item { get; private set; }

        public IRelationConfig Relation { get; private set; }

        public DatabaseParameterHandler Handler
        {
            get
            {
                var resolutionStrategy = new EntityPropertyResolutionStrategy<T>();
                return new DatabaseParameterHandler(parameters =>
                {
                    if (parameters.Contains(this.Relation.Name))
                    {
                        var property = resolutionStrategy.Resolve(Conventions.KeyColumn);
                        parameters[this.Relation.Name] = property.GetValue(this.Item);
                    }
                });
            }
        }
    }
}
