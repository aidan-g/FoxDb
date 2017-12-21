using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class TableConfig<T> : ITableConfig<T>
    {
        public TableConfig()
        {

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
            foreach (var property in PropertyEnumerator.GetProperties<T>())
            {
                this.Column(property.Name);
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
