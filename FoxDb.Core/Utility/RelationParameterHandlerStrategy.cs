using FoxDb.Interfaces;

namespace FoxDb
{
    public class RelationParameterHandlerStrategy<T, TRelation> : IParameterHandlerStrategy
        where T : IPersistable
        where TRelation : IPersistable
    {
        public RelationParameterHandlerStrategy(IDatabase database, T item, TRelation child, IRelationConfig relation)
        {
            this.Database = database;
            this.Item = item;
            this.Child = child;
            this.Relation = relation;
        }

        public IDatabase Database { get; private set; }

        public T Item { get; private set; }

        public TRelation Child { get; private set; }

        public IRelationConfig Relation { get; private set; }

        public DatabaseParameterHandler Handler
        {
            get
            {
                return new DatabaseParameterHandler(parameters =>
                {
                    if (this.Item != null && parameters.Contains(Conventions.RelationColumn(typeof(T))))
                    {
                        var resolver = new EntityPropertyResolutionStrategy<T>();
                        var property = resolver.Resolve(Conventions.KeyColumn);
                        parameters[Conventions.RelationColumn(typeof(T))] = property.GetValue(this.Item);
                    }
                    if (this.Child != null && parameters.Contains(Conventions.RelationColumn(typeof(TRelation))))
                    {
                        var resolver = new EntityPropertyResolutionStrategy<TRelation>();
                        var property = resolver.Resolve(Conventions.KeyColumn);
                        parameters[Conventions.RelationColumn(typeof(TRelation))] = property.GetValue(this.Child);
                    }
                });
            }
        }
    }
}
