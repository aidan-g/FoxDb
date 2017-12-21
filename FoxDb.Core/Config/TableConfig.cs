using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class TableConfig<T> : ITableConfig<T>
    {
        public TableConfig()
        {
            this.Name = Pluralization.Pluralize(typeof(T).Name);
            this.Columns = new Dictionary<string, IColumnConfig>();
            this.Relations = new Dictionary<Type, IRelationConfig>();
        }

        public string Name { get; set; }

        private IDictionary<string, IColumnConfig> Columns { get; set; }

        private IDictionary<Type, IRelationConfig> Relations { get; set; }

        IEnumerable<IColumnConfig> ITableConfig.Columns
        {
            get
            {
                return this.Columns.Values;
            }
        }

        IEnumerable<IRelationConfig> ITableConfig.Relations
        {
            get
            {
                return this.Relations.Values;
            }
        }

        public void UseDefaultColumns()
        {
            var resolutionStrategy = new EntityPropertyResolutionStrategy<T>();
            foreach (var property in resolutionStrategy.Properties)
            {
                var config = this.Column(property.Name);
                if (string.Equals(config.Name, Conventions.KEY_COLUMN, StringComparison.OrdinalIgnoreCase))
                {
                    config.IsKey = true;
                }
            }
        }

        public IColumnConfig Key
        {
            get
            {
                return this.Columns.Values.FirstOrDefault(column => column.IsKey);
            }
        }

        public IColumnConfig Column(string name)
        {
            if (!this.Columns.ContainsKey(name))
            {
                var config = new ColumnConfig(name);
                this.Columns.Add(name, config);
            }
            return this.Columns[name];
        }

        public IRelationConfig<T, TRelation> Relation<TRelation>(string name, Func<T, TRelation> selector)
        {
            var config = new RelationConfig<T, TRelation>(name, selector);
            this.Relations.Add(typeof(TRelation), config);
            return config;
        }

        public ICollectionRelationConfig<T, TRelation> Relation<TRelation>(string name, Func<T, ICollection<TRelation>> selector)
        {
            var config = new CollectionRelationConfig<T, TRelation>(name, selector);
            this.Relations.Add(typeof(TRelation), config);
            return config;
        }
    }
}
