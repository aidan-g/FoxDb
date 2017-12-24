using FoxDb.Interfaces;

namespace FoxDb
{
    public class ForeignKeysParameterHandlerStrategy<T, TRelation> : IParameterHandlerStrategy
        where T : IPersistable
        where TRelation : IPersistable
    {
        public ForeignKeysParameterHandlerStrategy(IDatabase database, T item, TRelation child, IRelationConfig relation)
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
                        var table = this.Database.Config.Table<T>();
                        parameters[Conventions.RelationColumn(typeof(T))] = table.PrimaryKey.Getter(this.Item);
                    }
                    if (this.Child != null && parameters.Contains(Conventions.RelationColumn(typeof(TRelation))))
                    {
                        var table = this.Database.Config.Table<TRelation>();
                        parameters[Conventions.RelationColumn(typeof(TRelation))] = table.PrimaryKey.Getter(this.Child);
                    }
                });
            }
        }
    }
}
