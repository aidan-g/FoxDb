using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public abstract class TableConfig : ITableConfig
    {
        protected TableConfig()
        {
            this.Columns = new Dictionary<string, IColumnConfig>();
            this.Relations = new Dictionary<Type, IRelationConfig>();
        }

        public string Name { get; set; }

        protected IDictionary<string, IColumnConfig> Columns { get; private set; }

        protected IDictionary<Type, IRelationConfig> Relations { get; private set; }

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

    }

    public class TableConfig<T> : TableConfig, ITableConfig<T> where T : IPersistable
    {
        public TableConfig(bool useDefaultColumns = true)
        {
            this.Name = Pluralization.Pluralize(typeof(T).Name);
            if (useDefaultColumns)
            {
                this.UseDefaultColumns();
            }
        }

        public ITableConfig<T> UseDefaultColumns()
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
            return this;
        }

        public IRelationConfig<T, TRelation> Relation<TRelation>(string name, Func<T, TRelation> getter, Action<T, TRelation> setter) where TRelation : IPersistable
        {
            var config = new RelationConfig<T, TRelation>(name, getter, setter);
            this.Relations.Add(typeof(TRelation), config);
            return config;
        }

        public ICollectionRelationConfig<T, TRelation> Relation<TRelation>(string name, Func<T, ICollection<TRelation>> getter, Action<T, ICollection<TRelation>> setter) where TRelation : IPersistable
        {
            var config = new CollectionRelationConfig<T, TRelation>(name, getter, setter);
            this.Relations.Add(typeof(TRelation), config);
            return config;
        }
    }
}
