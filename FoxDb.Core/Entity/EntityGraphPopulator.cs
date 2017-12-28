using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxDb
{
    public class EntityGraphPopulator : IEntityGraphPopulator
    {
        private EntityGraphPopulator()
        {
            this.Members = new DynamicMethod(this.GetType());
        }

        public EntityGraphPopulator(IDatabaseSet set) : this()
        {
            this.Set = set;
            this.Root = this.CreateNode(set.Table);
        }

        protected DynamicMethod Members { get; private set; }

        public IDatabaseSet Set { get; private set; }

        public EntityGraphNode Root { get; private set; }

        protected virtual EntityGraphNode CreateNode(ITableConfig table)
        {
            var node = this.Members.Invoke(this, "CreateNode", table.TableType, table);
            return (EntityGraphNode)node;
        }

        protected virtual EntityGraphNode<T> CreateNode<T>(ITableConfig<T> table) where T : class
        {
            return new EntityGraphNode<T>(this, table);
        }

        protected virtual EntityGraphNode CreateNode<T>(EntityGraphNode<T> parent, IRelationConfig relation) where T : class
        {
            var node = this.Members.Invoke(this, "CreateNode", new[] { parent.Table.TableType, relation.RelationType }, parent, relation);
            return (EntityGraphNode)node;
        }

        protected virtual EntityGraphNode<TRelation> CreateNode<T, TRelation>(EntityGraphNode<T> parent, IRelationConfig<T, TRelation> relation)
            where T : class
            where TRelation : class
        {
            return new EntityRelationGraphNode<T, TRelation>(this, parent, relation);
        }

        protected virtual EntityGraphNode<TRelation> CreateNode<T, TRelation>(EntityGraphNode<T> parent, ICollectionRelationConfig<T, TRelation> relation)
            where T : class
            where TRelation : class
        {
            return new EntityCollectionRelationGraphNode<T, TRelation>(this, parent, relation);
        }

        public IEnumerable<T> Populate<T>(IDatabaseReader reader)
        {
            var buffer = new List<T>();
            var sink = new EntityGraphSink<T>((sender, e) => buffer.Add(e.Item));
            foreach (var record in reader)
            {
                this.Root.Read(sink, record.ToDictionary());
                if (buffer.Count > 0)
                {
                    foreach (var item in buffer)
                    {
                        yield return item;
                    }
                    buffer.Clear();
                }
            }
            this.Root.Flush(sink);
            foreach (var item in buffer)
            {
                yield return item;
            }
        }

        public abstract class EntityGraphNode
        {
            protected EntityGraphNode(EntityGraphPopulator populator)
            {
                this.Populator = populator;
            }

            public EntityGraphPopulator Populator { get; private set; }

            public abstract void Read(IEntityGraphSink sink, IDictionary<string, object> data);

            public abstract void Flush(IEntityGraphSink sink);
        }

        public class EntityGraphNode<T> : EntityGraphNode where T : class
        {
            public EntityGraphNode(EntityGraphPopulator populator, ITableConfig<T> table) : base(populator)
            {
                this.Table = table;
                this.EntityFactory = new EntityFactory<T>();
                this.EntityInitializer = new EntityInitializer<T>(this.Table, this.Populator.Set.Mapper);
                this.EntityPopulator = new EntityPopulator<T>(this.Table, this.Populator.Set.Mapper);
                if (this.Populator.Set.Mapper.IncludeRelations)
                {
                    this.Children = GetChildren(populator, this);
                }
                else
                {
                    this.Children = Enumerable.Empty<EntityGraphNode>();
                }
            }

            public ITableConfig<T> Table { get; private set; }

            public IEntityFactory<T> EntityFactory { get; private set; }

            public IEntityInitializer<T> EntityInitializer { get; private set; }

            public IEntityPopulator<T> EntityPopulator { get; private set; }

            public IEnumerable<EntityGraphNode> Children { get; private set; }

            public T Item { get; private set; }

            public override void Read(IEntityGraphSink sink, IDictionary<string, object> data)
            {
                var column = this.Populator.Set.Mapper.GetColumn(this.Table.PrimaryKey);
                if (data.ContainsKey(column.Identifier))
                {
                    var key = data[column.Identifier];
                    if (!EntityKey<T>.IsKey(this.Populator.Set.Database, key))
                    {
                        return;
                    }
                    do
                    {
                        if (this.Item == null)
                        {
                            this.Item = this.CreateItem(data);
                            break;
                        }
                        else if (!EntityKey<T>.KeyEquals(this.Populator.Set.Database, this.Item, key))
                        {
                            this.Flush(sink);
                        }
                        else
                        {
                            break;
                        }
                    } while (true);
                }
                foreach (var child in this.Children)
                {
                    child.Read(null, data);
                }
            }

            protected virtual T CreateItem(IDictionary<string, object> data)
            {
                var item = this.EntityFactory.Create();
                this.EntityInitializer.Initialize(item);
                this.EntityPopulator.Populate(item, data);
                return item;
            }

            public override void Flush(IEntityGraphSink sink)
            {
                if (this.Item == null)
                {
                    return;
                }
                var target = sink as IEntityGraphSink<T>;
                if (target != null)
                {
                    target.Handler(this, new EntityGraphSinkEventArgs<T>(this.Item));
                }
                this.Item = null;
            }

            protected static IEnumerable<EntityGraphNode> GetChildren(EntityGraphPopulator populator, EntityGraphNode<T> parent)
            {
                foreach (var relation in parent.Table.Relations)
                {
                    if (!populator.Set.Mapper.Relations.Contains(relation))
                    {
                        continue;
                    }
                    yield return populator.CreateNode(parent, relation);
                }
            }
        }

        public class EntityRelationGraphNode<T, TRelation> : EntityGraphNode<TRelation>
            where T : class
            where TRelation : class
        {
            public EntityRelationGraphNode(EntityGraphPopulator populator, EntityGraphNode<T> parent, IRelationConfig<T, TRelation> relation) : base(populator, relation.Table)
            {
                this.Parent = parent;
                this.Relation = relation;
            }

            public EntityGraphNode<T> Parent { get; private set; }

            public IRelationConfig<T, TRelation> Relation { get; private set; }

            protected override TRelation CreateItem(IDictionary<string, object> data)
            {
                var item = base.CreateItem(data);
                this.Relation.Setter(this.Parent.Item, item);
                return item;
            }
        }

        public class EntityCollectionRelationGraphNode<T, TRelation> : EntityGraphNode<TRelation>
            where T : class
            where TRelation : class
        {
            public EntityCollectionRelationGraphNode(EntityGraphPopulator populator, EntityGraphNode<T> parent, ICollectionRelationConfig<T, TRelation> relation) : base(populator, relation.Table)
            {
                this.Parent = parent;
                this.Relation = relation;
            }

            public EntityGraphNode<T> Parent { get; private set; }

            public ICollectionRelationConfig<T, TRelation> Relation { get; private set; }

            protected override TRelation CreateItem(IDictionary<string, object> data)
            {
                var item = base.CreateItem(data);
                var sequence = this.Relation.Getter(this.Parent.Item);
                if (sequence == null)
                {
                    sequence = Factories.CollectionFactory.Create<TRelation>(new[] { item });
                    this.Relation.Setter(this.Parent.Item, sequence);
                }
                else
                {
                    sequence.Add(item);
                }
                return item;
            }
        }
    }
}
