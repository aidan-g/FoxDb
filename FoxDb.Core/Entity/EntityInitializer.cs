using FoxDb.Interfaces;
using System.Linq;

namespace FoxDb
{
    public class EntityInitializer<T> : IEntityInitializer<T>
    {
        private EntityInitializer()
        {
            this.Members = new DynamicMethod(this.GetType());
        }

        public EntityInitializer(ITableConfig table, IEntityMapper mapper) : this()
        {
            this.Table = table;
            this.Mapper = mapper;
        }

        protected DynamicMethod Members { get; private set; }

        public ITableConfig Table { get; private set; }

        public IEntityMapper Mapper { get; private set; }

        public void Initialize(T item)
        {
            if (!this.Mapper.IncludeRelations)
            {
                return;
            }
            foreach (var relation in this.Table.Relations)
            {
                if (!this.Mapper.Relations.Contains(relation))
                {
                    continue;
                }
                this.Members.Invoke(this, "Initialize", new[] { typeof(T), relation.RelationType }, item, relation);
            }
        }

        protected virtual void Initialize<TEntity, TRelation>(TEntity item, IRelationConfig<TEntity, TRelation> relation)
        {
            //Nothing to do.
        }

        protected virtual void Initialize<TEntity, TRelation>(TEntity item, ICollectionRelationConfig<TEntity, TRelation> relation)
        {
            relation.Setter(item, Factories.CollectionFactory.Create<TRelation>());
        }
    }
}
