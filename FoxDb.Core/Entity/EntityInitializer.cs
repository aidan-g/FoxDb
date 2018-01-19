using FoxDb.Interfaces;

namespace FoxDb
{
    public class EntityInitializer : IEntityInitializer
    {
        private EntityInitializer()
        {
            this.Members = new DynamicMethod(this.GetType());
        }

        public EntityInitializer(ITableConfig table) : this()
        {
            this.Table = table;
        }

        protected DynamicMethod Members { get; private set; }

        public ITableConfig Table { get; private set; }

        public void Initialize(object item)
        {
            foreach (var relation in this.Table.Relations)
            {
                this.Members.Invoke(this, "Initialize", new[] { this.Table.TableType, relation.RelationType }, item, relation);
            }
        }

        protected virtual void Initialize<TEntity, TRelation>(TEntity item, IRelationConfig<TEntity, TRelation> relation)
        {
            //Nothing to do.
        }

        protected virtual void Initialize<TEntity, TRelation>(TEntity item, ICollectionRelationConfig<TEntity, TRelation> relation)
        {
            relation.Setter(item, Factories.Collection.Create<TRelation>(relation.Property.PropertyType));
        }
    }
}
